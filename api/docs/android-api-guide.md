# SporeAccounting 安卓客户端接口文档

> 面向 Android/Kotlin 客户端开发，根据当前控制器、DTO、网关 `ocelot.json` 和认证中间件整理，更新时间：2026-08-29。
> 完整响应字段可同时参考 [API 输出参考](./api-response-reference.md)。

## 1. 接入原则

### 1.1 Base URL

安卓客户端应只访问 API Gateway，不应直连各微服务。

```text
开发配置中的网关地址：http://localhost:9000/
```

Android 模拟器不能用 `localhost` 访问宿主机：

- Android Studio Emulator：使用 `http://10.0.2.2:9000/`。
- 真机：使用电脑局域网 IP 或部署后的 HTTPS 域名。
- 正式环境必须使用 HTTPS；Android 9 及以上默认禁止明文 HTTP。

网关实际地址可能由部署配置覆盖，客户端应通过 `BuildConfig.API_BASE_URL` 区分环境。

### 1.2 请求头

除 Nacos `NoAuthPaths` 配置明确放行的接口外，网关默认要求：

```http
Authorization: Bearer <access_token>
Accept: application/json
Content-Type: application/json
```

客户端不要自行发送 `X-User-Id`、`X-User-Name`、`X-User-Email`、`X-Gateway-Signature`，这些由网关生成。

匿名路径来自运行时 Nacos 配置，不完全由代码注解决定。当前仓库示例至少放行 `/api/auth/token`，但示例中的注册/验证码路径与实际控制器不一致，联调前应确认部署环境的 `NoAuthPaths`。

### 1.3 JSON 与 Kotlin 类型

| 后端类型 | JSON | Kotlin 建议类型 |
| --- | --- | --- |
| `long` | integer | `Long` |
| `int` | integer | `Int` |
| `decimal` | number | 金额优先 `BigDecimal`，或统一约定后使用 `Double` |
| `float` | number | `Float` |
| `DateTime` | ISO 8601 string | `String`，或配置转换器后使用 `Instant`/`LocalDateTime` |
| `T?` | value/null | Kotlin nullable 类型 `T?` |
| C# enum | integer | `Int` 或带数值适配器的 Kotlin enum |
| `Ok()` | 空响应体 | Retrofit 使用 `Response<Unit>`，不要声明 `Boolean` |

字段名为 `camelCase`。当前枚举按整数传输，不能直接发送枚举名称字符串。

### 1.4 通用分页结构

```kotlin
data class PageResponse<T>(
    val totalCount: Int,
    val data: List<T>,
    val pageIndex: Int,
    val pageSize: Int,
    val totalPage: Int,
)
```

Finance/Common 分页请求使用 `page`、`size` 或请求体中的 `pageIndex`、`pageSize`；Identity 分页查询使用 `page`、`pageSize`、`sortField`、`sortOrder`。接口表中已分别注明。

### 1.5 错误结构

下游业务错误经过网关后通常被替换为：

```json
{
  "statusCode": 400,
  "errorMessage": "下游服务返回错误",
  "stackTrace": null
}
```

网关认证错误直接返回：

```json
{
  "error": "invalid_token",
  "error_description": "缺少有效的访问令牌"
}
```

建议客户端兼容两种结构：

```kotlin
data class ApiError(
    val statusCode: Int? = null,
    val errorMessage: String? = null,
    val stackTrace: String? = null,
    val errors: Map<String, List<String>>? = null,
    val error: String? = null,
    @SerializedName("error_description")
    val errorDescription: String? = null,
)
```

处理建议：

- `400`：显示输入错误，保留表单内容。
- `401`：尝试刷新令牌；失败则清除本地凭据并跳转登录页。
- `403`：提示无权限，不要重试。
- `404`：区分数据不存在与网关未发布路由。
- `502` / `503` / `504`：服务暂不可用，允许用户重试。
- Retrofit 的 `response.isSuccessful` 为 `true` 不代表响应体一定存在；空响应接口必须按 `Unit` 处理。

## 2. Retrofit 建议配置

```kotlin
interface TokenStore {
    fun accessToken(): String?
}

class BearerInterceptor(
    private val tokenStore: TokenStore,
) : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        val token = tokenStore.accessToken()
        val request = chain.request().newBuilder().apply {
            header("Accept", "application/json")
            if (!token.isNullOrBlank()) {
                header("Authorization", "Bearer $token")
            }
        }.build()
        return chain.proceed(request)
    }
}
```

```kotlin
val okHttp = OkHttpClient.Builder()
    .addInterceptor(BearerInterceptor(tokenStore))
    .connectTimeout(15, TimeUnit.SECONDS)
    .readTimeout(35, TimeUnit.SECONDS) // 网关下游超时为 30 秒
    .build()

val retrofit = Retrofit.Builder()
    .baseUrl(BuildConfig.API_BASE_URL) // 必须以 / 结尾
    .client(okHttp)
    .addConverterFactory(GsonConverterFactory.create())
    .build()
```

令牌刷新需要通过单例 `Authenticator` 串行执行，避免多个并发 401 同时刷新。刷新接口仍为 `/api/auth/token`，请求格式是表单而不是 JSON。

## 3. 登录与验证码

### 3.1 获取令牌

`POST /api/auth/token`

请求类型：`application/x-www-form-urlencoded`。

密码登录：

```kotlin
@FormUrlEncoded
@POST("api/auth/token")
suspend fun login(
    @Field("grant_type") grantType: String = "password",
    @Field("username") username: String,
    @Field("password") password: String,
    @Field("scope") scope: String = "api offline_access",
): TokenResponse
```

刷新令牌：

```kotlin
@FormUrlEncoded
@POST("api/auth/token")
suspend fun refresh(
    @Field("grant_type") grantType: String = "refresh_token",
    @Field("refresh_token") refreshToken: String,
    @Field("scope") scope: String = "api offline_access",
): TokenResponse
```

```kotlin
data class TokenResponse(
    @SerializedName("token_type") val tokenType: String,
    @SerializedName("access_token") val accessToken: String,
    @SerializedName("expires_in") val expiresIn: Long,
    @SerializedName("refresh_token") val refreshToken: String? = null,
    val scope: String? = null,
)
```

还支持 `client_credentials`、`sms_otp`、`email_code`，但客户端凭证不应内置在 APK 中；短信/邮箱验证码发送接口目前未被网关发布，因此移动端暂不应启用验证码登录。

敏感参数必须放在表单请求体，不能拼到 URL query。

### 3.2 图片验证码

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `GET /api/captcha/create` | 无 | `CaptchaCreateResponse` |
| `POST /api/captcha/verify` | JSON `CaptchaVerifyRequest` | `{ "success": true }` |

```kotlin
data class CaptchaCreateResponse(
    val token: String,
    val imageBase64: String,
    val expiresInSeconds: Int,
)

data class CaptchaVerifyRequest(
    val token: String,
    val code: String,
)

data class SuccessResponse(val success: Boolean)
```

`imageBase64` 可能包含 data URL 前缀；展示前应兼容有/无 `data:image/...;base64,` 两种形式。

## 4. 用户、角色与客户端

### 4.1 用户

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `GET /api/users/{id}` | path `id: Long` | `UserResponse` |
| `GET /api/users` | query `page, pageSize, sortField, sortOrder, userName, email` | `PageResponse<UserResponse>` |
| `PUT /api/users/{id}` | `UserUpdateRequest` | 空 |
| `PUT /api/users/{id}/status` | JSON 原始 boolean | 空 |
| `DELETE /api/users/{id}` | path `id` | 空 |
| `GET /api/users/phone/verify` | 无 | boolean |
| `GET /api/users/email/verify` | 无 | boolean |

```kotlin
data class UserResponse(
    val id: Long,
    val userName: String,
    val email: String,
    val phoneNumber: String,
    val isLocked: Boolean,
)

data class UserUpdateRequest(
    val userName: String,
    val email: String,
    val phoneNumber: String?,
)
```

状态更新的 body 是 JSON 字面量 `true`/`false`，不是 `{ "isDisabled": true }`。

### 4.2 角色

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `GET /api/roles` | query `page, pageSize, sortField, sortOrder, roleName` | `PageResponse<RoleResponse>` |
| `GET /api/roles/{id}` | path `id` | `RoleResponse` |
| `POST /api/roles` | `{ "roleName": "..." }` | 空 |
| `PUT /api/roles/{id}` | `{ "roleName": "..." }` | 空 |
| `DELETE /api/roles/{id}` | path `id` | 空 |
| `POST /api/roles/{roleId}/permissions` | JSON 原始字符串 | 空 |
| `GET /api/roles/{roleId}/permissions` | path `roleId` | `String[]` |
| `DELETE /api/roles/{roleId}/permissions/{permission}` | path 参数 | 空 |
| `GET /api/roles/users/{userId}/permissions` | path `userId` | `String[]` |

权限新增 body 必须是合法 JSON 字符串，例如 `"budget.read"`，不能发送 `{ "permission": "budget.read" }`。

### 4.3 OAuth 客户端管理

网关当前只发布：

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/clients/register` | `ClientRegistrationRequest` | `{ success, message, client_id }` |
| `DELETE /api/clients/{clientId}` | path `clientId` | `{ success, message, client_id }` |

该模块属于管理功能，不建议普通安卓用户端开放。客户端密钥不可硬编码进 APK。

## 5. 账本与分享

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/account-books` | `AccountBookAddRequest` | 新账本 ID `Long` |
| `GET /api/account-books` | query `page=1, size=10` | `PageResponse<AccountBookResponse>` |
| `PUT /api/account-books/{id}` | `AccountBookEditRequest` | 空 |
| `DELETE /api/account-books/{id}` | path `id` | 空 |
| `POST /api/account-books/merge` | `AccountBookMergeRequest` | 空 |
| `POST /api/account-books/share` | `AccountBookShareAddRequest` | 空 |
| `PUT /api/account-books/share` | `AccountBookShareEditRequest` | 空 |
| `PUT /api/account-books/share/Revoke` | `AccountBookRevokeSharingRequest` | 空 |
| `POST /api/account-books/share/Page/Self` | `PageBody` | `PageResponse<AccountBookShareResponse>` |
| `POST /api/account-books/share/Page/SharedToMe` | `PageBody` | `PageResponse<AccountBookShareResponse>` |

```kotlin
data class PageBody(val pageIndex: Int = 1, val pageSize: Int = 10)

data class AccountBookAddRequest(
    val name: String,
    val remarks: String? = null,
)

data class AccountBookEditRequest(
    val id: Long,
    val name: String,
    val remarks: String? = null,
)

data class AccountBookMergeRequest(
    val targetAccountBookId: Long,
    val sourceAccountBookIds: List<Long>,
)

data class AccountBookShareAddRequest(
    val accountBookId: Long,
    val userId: List<Long>, // 后端字段是单数 userId，但类型为数组
    val permissionType: Int,
)

data class AccountBookShareEditRequest(
    val accountBookId: Long,
    val userIds: List<Long>,
    val permissionType: Int,
)

data class AccountBookRevokeSharingRequest(
    val accountBookId: Long,
    val userIds: List<Long>,
)
```

`permissionType`：`0` 只读，`1` 读写，`2` 管理。

## 6. 记账

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/accountings` | `AccountingAddRequest` | 新记录 ID `Long` |
| `GET /api/accountings/{id}` | path `id` + query `accountBookId` | `AccountingResponse` |
| `GET /api/accountings` | query 见下方 | `PageResponse<AccountingResponse>` |
| `PUT /api/accountings/{id}` | `AccountingEditRequest` | boolean |
| `DELETE /api/accountings/{id}` | path `id` + query `accountBookId` | boolean |
| `GET /api/accountings/by-time-range` | query `startTime, endTime` | `AccountingResponse[]` |

列表 query：`accountBookId: Long`、`page: Int=1`、`size: Int=10`、`startTime: DateTime?`、`endTime: DateTime?`、`categoryId: Long?`。

```kotlin
data class AccountingAddRequest(
    val amount: BigDecimal,
    val transactionCategoryId: Long,
    val accountBookId: Long,
    val recordDate: String,
    val currencyId: Long,
    val remark: String? = null,
    val paymentMethodId: Long,
)

data class AccountingEditRequest(
    val id: Long,
    val amount: BigDecimal,
    val transactionCategoryId: Long,
    val accountBookId: Long,
    val recordDate: String,
    val currencyId: Long,
    val remark: String? = null,
    val paymentMethodId: Long,
)
```

更新时 body 的 `id` 必须与 path `{id}` 相同。创建模型的字段在 C# 中可空，但服务端使用前会解引用，安卓端应全部填写（仅 `remark` 可空）。

## 7. 预算与财务健康

### 7.1 预算

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/budgets` | `BudgetRequest` | 新预算 ID `Long` |
| `GET /api/budgets` | query `page, size, year, month` | `PageResponse<BudgetResponse>` |
| `GET /api/budgets/{id}` | path `id` | `BudgetResponse` |
| `GET /api/budgets/current-budgets` | 无 | `BudgetResponse[]` |
| `PUT /api/budgets/{id}` | `BudgetEditRequest` | boolean |
| `DELETE /api/budgets/{id}` | path `id` | boolean |
| `GET /api/budget-records/by-budget-ids` | 无 | `Map<String, List<BudgetRecordResponse>>` |

```kotlin
data class BudgetRequest(
    val transactionCategoryId: Long,
    val amount: BigDecimal,
    val period: Int,
    val remark: String? = null,
    val startTime: String,
    val endTime: String,
)

data class BudgetEditRequest(
    val id: Long,
    val transactionCategoryId: Long,
    val amount: BigDecimal,
    val period: Int,
    val remark: String? = null,
    val startTime: String,
    val endTime: String,
)
```

`period`：`0` 年，`1` 月，`2` 季度。更新 body 的 `id` 必须等于 path `{id}`。

预算记录响应是 JSON object，数字 key 在 JSON 中仍为字符串，例如：

```json
{
  "1001": [
    {
      "budgetId": 1001,
      "recordDate": "2026-08-29T00:00:00",
      "usedAmount": 28.50,
      "period": 1,
      "transactionCategoryId": 8
    }
  ]
}
```

### 7.2 财务健康

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/financial-health/calculate` | query `accountBookId, periodStart, periodEnd` | `FinancialHealthScoreResponse` |
| `GET /api/financial-health/score` | query `accountBookId` | `FinancialHealthScoreResponse?` |
| `GET /api/financial-health/history` | query `accountBookId, page=1, size=10` | `PageResponse<FinancialHealthScoreResponse>` |
| `GET /api/financial-health/suggestions` | query `accountBookId` | `FinancialSuggestionResponse[]` |
| `GET /api/health` | 无 | JSON 字符串 `"Healthy"` |

`score` 返回 HTTP 200 + `null` 表示尚无评分，不应当作网络错误。

## 8. 分类、币种、汇率与支付方式

### 8.1 收支分类

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/transaction-categories` | `TransactionCategoryAddRequest` | 新分类 ID `Long` |
| `GET /api/transaction-categories/by-parent/{parentId}` | path `parentId` | `TransactionCategoryResponse[]` |
| `PUT /api/transaction-categories/{id}` | `{ id, name }` | boolean |
| `PUT /api/transaction-categories/update-parent` | `{ id: Long[]?, parentId: Long }` | boolean |
| `DELETE /api/transaction-categories/batch` | JSON `Long[]` | boolean |

```kotlin
data class TransactionCategoryAddRequest(
    val name: String,
    val parentId: Long?,
    val type: Int, // 0 收入，1 支出
)
```

### 8.2 币种与汇率

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `GET /api/currencies` | 无 | `CurrencyResponse[]` |
| `GET /api/exchange-rates` | query `sourceCurrencyId, targetCurrencyId, page=1, size=10` | `PageResponse<ExchangeRateRecordResponse>` |
| `GET /api/exchange-rates/{sourceCurrencyId}/{targetCurrencyId}/today` | path 参数 | `ExchangeRateRecordResponse` |

### 8.3 支付方式

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/payment-methods` | `PaymentMethodRequest` | 新支付方式 ID `Long` |
| `GET /api/payment-methods` | 无 | `PaymentMethodResponse[]` |
| `PUT /api/payment-methods/{id}` | `PaymentMethodEditRequest` | boolean |
| `DELETE /api/payment-methods/{id}` | path `id` | boolean |
| `PATCH /api/payment-methods/{id}/default` | path `id` | boolean |

```kotlin
data class PaymentMethodRequest(
    val name: String,
    val type: Int,
    val electronicPaymentType: Int?,
    val isDefault: Boolean = false,
    val remark: String? = null,
)
```

`type`：`0` 现金，`1` 信用卡，`2` 电子支付；`electronicPaymentType`：`0` 支付宝，`1` 微信，`2` 银联，`3` 其他。

## 9. 周期性支出

当前接口存在路由冲突，不建议安卓端接入：

- 新增与删除都注册为 `POST /api/recurring-expense-rule`，请求时可能产生 `AmbiguousMatchException`。
- 单条查询声明从 route 读取 `id`，但 route 中没有 `{id}`。
- 分页路径实际为根路径 `POST /page`，而非 `/api/recurring-expense-rule/page`。

网关虽然发布了这些路径，客户端仍应等待后端修复后再实现功能。

## 10. 分摊支出

### 10.1 分摊项目

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/shared-expenses` | `SharedExpenseRequest` | 新项目 ID `Long` |
| `GET /api/shared-expenses` | query `accountBookId` | `SharedExpenseResponse[]` |
| `GET /api/shared-expenses/{id}` | path `id` | `SharedExpenseResponse` |
| `PUT /api/shared-expenses/{id}` | `SharedExpenseEditRequest` | boolean |
| `DELETE /api/shared-expenses/{id}` | path `id` | boolean |

```kotlin
data class SharedExpenseRequest(
    val accountBookId: Long,
    val title: String,
    val totalAmount: BigDecimal,
    val currencyId: Long,
    val transactionCategoryId: Long,
    val expenseDate: String,
    val splitType: Int,
    val participants: List<SharedExpenseParticipantRequest>,
    val description: String? = null,
)

data class SharedExpenseParticipantRequest(
    val participantId: Long,
    val shareAmount: BigDecimal,
    val shareRatio: BigDecimal? = null,
    val remark: String? = null,
)
```

`splitType`：`0` 按比例，`1` 人均，`2` 固定金额，`3` 混合。编辑请求在上述字段外增加 `id: Long`，且必须等于 path `{id}`。

### 10.2 提醒与结算

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/shared-expense-reminders` | `SharedExpenseReminderRequest` | 新提醒 ID `Long` |
| `GET /api/shared-expense-reminders/by-shared-expense/{sharedExpenseId}` | path 参数 | `SharedExpenseReminderResponse[]` |
| `PUT /api/shared-expense-reminders/{id}/status` | `{ id, status }` | boolean |
| `POST /api/shared-expense-settlements` | `SharedExpenseSettlementRequest` | 新结算 ID `Long` |
| `GET /api/shared-expense-settlements/{id}` | path `id` | `SharedExpenseSettlementResponse` |
| `GET /api/shared-expense-settlements/by-shared-expense/{sharedExpenseId}` | path 参数 | `SharedExpenseSettlementResponse[]` |

提醒类型：`0` 未支付，`1` 逾期，`2` 手动。提醒状态：`0` 待发送，`1` 已发送，`2` 失败，`3` 已读，`4` 已处理。

```kotlin
data class SharedExpenseReminderRequest(
    val sharedExpenseId: Long,
    val participantId: Long,
    val reminderType: Int,
    val content: String?,
    val scheduledTime: String,
    val nextReminderTime: String?,
)

data class SharedExpenseSettlementRequest(
    val sharedExpenseId: Long,
    val participantId: Long,
    val receiverId: Long,
    val paymentAmount: BigDecimal,
    val paymentMethod: String?,
    val settlementDate: String,
    val proofUrl: String?,
    val remark: String?,
)
```

## 11. 报表

网关当前发布：

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/Report/GetReport` | `{ reportType, year, month? }` | `ReportResponse[]` |
| `GET /api/budget-reports/budget-progress` | 无 | `BudgetProgressReportResponse[]` |
| `GET /api/budget-reports/budget-consumption-trend` | 无 | `BudgetConsumptionTrendReportResponse[]` |

`reportType`：`0` 月报，`1` 季报，`2` 年报。路径中的 `Report` 大小写按网关配置保留。

`budget-insight` 和 `GetReportInsight` 已有后端控制器，但尚未加入网关，因此客户端暂不可调用。

## 12. 文件、OCR 与智能提取

### 12.1 服务端上传

`POST /api/files/upload?isPublic=true`，请求为 `multipart/form-data`，字段名必须为 `file`，成功响应为空。

```kotlin
@Multipart
@POST("api/files/upload")
suspend fun upload(
    @Part file: MultipartBody.Part,
    @Query("isPublic") isPublic: Boolean = true,
): Response<Unit>
```

### 12.2 客户端直传

推荐流程：

1. `GET /api/files/upload-token?fileName=avatar.jpg&isPublic=true` 获取预签名 PUT URL。
2. 使用独立、不带 Gateway Bearer 拦截器的 OkHttpClient，向 `uploadUrl` 执行 PUT。
3. `POST /api/files/confirm-upload` 确认并获取 `fileId`。
4. `GET /api/files/url?fileId=...` 获取访问 URL。

确认请求：

```kotlin
data class ConfirmUploadRequest(
    val objectName: String,
    val isPublic: Boolean,
    val originalFileName: String,
    val contentType: String,
    val fileSize: Long,
)
```

注意：后端 `PresignedURLResponse` 当前使用公共字段而非属性，默认 System.Text.Json 可能实际返回 `{}`。修复后预期字段是 `uploadUrl`、`objectName`、`originalFileName`；修复前安卓端不能依赖直传流程。

其他文件接口：

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `GET /api/files/url` | query `fileId` | JSON string URL |
| `DELETE /api/files` | query `fileId` | 空 |
| `GET /api/ocr/recognize` | query `fileId` | 空 |
| `GET /api/ocr/text` | query `fileId` | string/null |

OCR 当前是两个步骤：先调用 `recognize`，成功后再调用 `text`。识别可能耗时，UI 应显示进度并允许重试。

### 12.3 金额与分类提取

`POST /api/assistant/extract-amount-and-category`

请求体是 JSON 原始字符串，例如：

```json
"午餐花了28元"
```

不是 `{ "text": "午餐花了28元" }`。响应：

```kotlin
data class AmountAndCategoryExtractionResponse(
    val category: String?,
    val amount: BigDecimal?,
    val currency: String?,
)
```

## 13. 站内通知

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/insite-notifications` | `{ userId?, title, content }` | 通知 ID `Long` |
| `POST /api/insite-notifications/all` | `{ userId?, title, content }` | 空 |
| `GET /api/insite-notifications` | query `pageIndex, pageSize` | `PageResponse<InSiteNotificationResponse>` |
| `GET /api/insite-notifications/{notificationId}` | path 参数 | `InSiteNotificationResponse` |
| `GET /api/insite-notifications/unread-count` | 无 | `Int` |
| `PUT /api/insite-notifications/{notificationId}/read` | 无 body | 空 |
| `PUT /api/insite-notifications` | `{ id, title, content }` | 空 |
| `DELETE /api/insite-notifications` | JSON `Long[]` | 空 |

```kotlin
data class InSiteNotificationResponse(
    val id: Long,
    val title: String,
    val content: String,
    val isRead: Boolean,
    val createdAt: String,
)
```

群发、编辑等能力建议仅在管理端开放。

## 14. 分类预测 ML

路径中的 `CategoryPrediction` 大小写按网关配置保留。

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `POST /api/CategoryPrediction/predict` | `PredictionRequest` | `PredictionResponse` |
| `POST /api/CategoryPrediction/feedback/choice` | `ChoiceFeedbackRequest` | `{ message }` |
| `POST /api/CategoryPrediction/feedback/correction` | `CorrectionFeedbackRequest` | `{ message }` |
| `GET /api/CategoryPrediction/stats` | 无 | `LearningStatsResponse` |
| `POST /api/CategoryPrediction/retrain` | 无 | `{ message }` |

```kotlin
data class CategoryDto(val id: String, val name: String)

data class PredictionRequest(
    val query: String,
    val categories: List<CategoryDto>,
    val merchant: String,
    val amountBucket: Float,
    val hourOfDay: Float,
)

data class PredictionResponse(
    val predictedCategory: CategoryDto,
    val confidence: Float,
    val method: String,
    val requiresUserConfirmation: Boolean,
)
```

选择反馈使用 `query`、`selectedCategory`、`availableCategories`、`merchant`、`amountBucket`、`hourOfDay`；纠错反馈将 `selectedCategory` 替换为 `wrongCategory` 和 `correctCategory`。

`retrain` 属于管理操作，普通移动端不应展示。

## 15. 用户配置

| 方法与路径 | 请求 | 成功响应 |
| --- | --- | --- |
| `GET /api/configs` | 无 | `ConfigResponse[]` |
| `PUT /api/configs` | `{ id, configType, value }` | boolean |
| `GET /api/configs/by-type/{type}` | path enum integer | `ConfigResponse` |
| `GET /api/configs/by-type-and-user/{type}/{userId}` | path 参数 | `ConfigResponse` |

`by-type-and-user` 只允许查询当前登录用户自己的配置。

## 16. 后端存在但网关未发布的接口

下列控制器动作无法通过当前 Gateway `ocelot.json` 调用，安卓端不应直连微服务绕过网关：

| 模块 | 未发布路径 |
| --- | --- |
| 注册/认证 | `/api/auth/register`、`emailVerificationCode`、`smsVerificationCode`、`email/bind`、`phone/bind`、`password/reset`、`logout`、`connect/logout`、`revoke`、`userinfo`、`introspect` |
| OAuth 客户端 | `GET /api/clients`、`GET /api/clients/{clientId}/exists`、`POST /api/clients/initialize` |
| 预算生成 | `/api/budget-generation`、`/api/budget-generation/{id}/confirm`；同时后端尚未实现 |
| 财务健康 | `GET /api/financial-health/suggestions/all` |
| 报表解读 | `GET /api/budget-reports/budget-insight`、`POST /api/Report/GetReportInsight` |
| AI 使用记录 | `GET/POST /api/ai-usage-record...` |

若移动端需求包含注册、找回密码或退出登录，必须先由后端补充网关路由并同步 `NoAuthPaths`，否则客户端无法完成完整账号闭环。

## 17. 联调前检查清单

- 确认正式 Gateway HTTPS Base URL。
- 确认 Nacos `NoAuthPaths` 与实际登录、注册、验证码路径一致。
- 补齐移动端必需但网关缺失的认证路由。
- 修复 `PresignedURLResponse` 序列化后再接入预签名直传。
- 修复周期性支出重复/错误路由后再开发该模块。
- 明确金额使用 `BigDecimal` 还是 `Double`，并在全客户端统一。
- 明确 DateTime 是否统一 UTC；传输时建议携带 `Z` 或时区偏移。
- 对所有空响应接口使用 `Response<Unit>`。
- 对 HTTP 200 + `null`（如最新财务评分）单独建模。
- 401 刷新令牌使用单飞机制，避免并发刷新和循环重试。
- 不在日志、崩溃上报或抓包持久化中记录密码、access token、refresh token、客户端密钥。
