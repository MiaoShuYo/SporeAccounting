# API 输出参考

> 根据当前仓库控制器与响应模型静态整理，更新时间：2026-08-29。
> 本文只描述接口输出；请求参数、鉴权方式和业务规则请以 Swagger/OpenAPI 与代码为准。

## 1. 通用约定

- ASP.NET Core 默认将 C# 属性名序列化为 `camelCase`，例如 `TotalCount` 输出为 `totalCount`。
- `DateTime` 通常输出为 ISO 8601 字符串，例如 `"2026-08-29T12:00:00Z"`。
- 当前未配置字符串枚举转换器，因此枚举默认输出为整数。
- `Ok()` 为 HTTP 200、空响应体；`Ok(value)` 为 HTTP 200、响应体直接是 `value`，没有统一的 `code/data/message` 外层包装。
- 业务异常通常由全局异常中间件转换为 `ExceptionResponse`；少数控制器会直接返回字符串或匿名错误对象。

### 1.1 分页输出 `PageResponse<T>`

```json
{
  "totalCount": 100,
  "data": [],
  "pageIndex": 1,
  "pageSize": 10,
  "totalPage": 10
}
```

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `totalCount` | integer | 总记录数 |
| `data` | `T[]` | 当前页数据 |
| `pageIndex` | integer | 页码 |
| `pageSize` | integer | 每页数量 |
| `totalPage` | integer | 总页数 |

### 1.2 全局异常输出 `ExceptionResponse`

```json
{
  "statusCode": 400,
  "errorMessage": "错误信息",
  "stackTrace": null
}
```

`stackTrace` 只应在调试环境出现。控制器自行返回的错误不一定使用此结构。

## 2. Config Service

| 方法与路径 | 成功输出 |
| --- | --- |
| `GET /api/configs` | `ConfigResponse[]` |
| `PUT /api/configs` | `true` |
| `GET /api/configs/by-type/{type}` | `ConfigResponse` |
| `GET /api/configs/by-type-and-user/{type}/{userId}` | `ConfigResponse` |

`ConfigResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 配置 ID |
| `value` | string | 配置值 |

## 3. Currency Service

| 方法与路径 | 成功输出 |
| --- | --- |
| `GET /api/currencies` | `CurrencyResponse[]` |
| `GET /api/exchange-rates` | `PageResponse<ExchangeRateRecordResponse>` |
| `GET /api/exchange-rates/{sourceCurrencyId}/{targetCurrencyId}/today` | `ExchangeRateRecordResponse` |

`CurrencyResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 币种 ID |
| `name` | string | 币种名称 |
| `abbreviation` | string | 币种缩写 |

`ExchangeRateRecordResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 汇率记录 ID |
| `exchangeRate` | number(decimal) | 汇率 |
| `convertCurrency` | string | 币种转换描述 |
| `date` | string(date-time) | 汇率日期 |

## 4. Finance Service

### 4.1 账本

| 方法与路径 | 成功输出 | 当前实现备注 |
| --- | --- | --- |
| `POST /api/account-books` | integer(int64) | 新账本 ID |
| `DELETE /api/account-books/{id}` | 空 | 声明为 `bool`，实际 `Ok()` |
| `PUT /api/account-books/{id}` | 空 | 声明为 `bool`，实际 `Ok()` |
| `GET /api/account-books` | `PageResponse<AccountBookResponse>` |  |
| `POST /api/account-books/merge` | 空 | 声明为 `bool`，实际 `Ok()` |
| `POST /api/account-books/share` | 空 | 声明为 `bool`，实际 `Ok()` |
| `POST /api/account-books/share/Page/Self` | `PageResponse<AccountBookShareResponse>` | 路径大小写按代码保留 |
| `POST /api/account-books/share/Page/SharedToMe` | `PageResponse<AccountBookShareResponse>` | 路径大小写按代码保留 |
| `PUT /api/account-books/share/Revoke` | 空 | 声明为 `bool`，实际 `Ok()` |
| `PUT /api/account-books/share` | 空 | 声明为 `bool`，实际 `Ok()` |

`AccountBookResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 账本 ID |
| `name` | string | 名称 |
| `remarks` | string/null | 备注 |
| `incomeAmount` | number(decimal) | 收入金额 |
| `expenditureAmount` | number(decimal) | 支出金额 |
| `permissionType` | integer(enum) | `0` 只读，`1` 读写，`2` 管理 |

`AccountBookShareResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 分享 ID |
| `accountBookId` | integer(int64) | 账本 ID |
| `userId` | integer(int64) | 用户 ID |
| `permissionType` | integer(enum) | `0` 只读，`1` 读写，`2` 管理 |

### 4.2 记账

| 方法与路径 | 成功输出 |
| --- | --- |
| `POST /api/accountings` | integer(int64)，新记录 ID |
| `DELETE /api/accountings/{id}` | boolean |
| `PUT /api/accountings/{id}` | boolean |
| `GET /api/accountings/{id}` | `AccountingResponse` |
| `GET /api/accountings` | `PageResponse<AccountingResponse>` |
| `GET /api/accountings/by-time-range` | `AccountingResponse[]` |

`AccountingResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 记账记录 ID |
| `amount` | number(decimal) | 金额 |
| `transactionCategoryId` | integer(int64) | 收支分类 ID |
| `transactionCategoryName` | string | 收支分类名称 |
| `recordDate` | string(date-time) | 记录日期 |
| `currencyId` | integer(int64) | 币种 ID |
| `currencyName` | string | 币种名称 |
| `remark` | string/null | 备注 |
| `paymentMethodId` | integer(int64) | 支付方式 ID |
| `paymentMethodName` | string | 支付方式名称 |

### 4.3 预算

| 方法与路径 | 成功输出 | 当前实现备注 |
| --- | --- | --- |
| `POST /api/budgets` | integer(int64)，新预算 ID |  |
| `DELETE /api/budgets/{id}` | boolean |  |
| `PUT /api/budgets/{id}` | boolean |  |
| `GET /api/budgets` | `PageResponse<BudgetResponse>` |  |
| `GET /api/budgets/{id}` | `BudgetResponse` |  |
| `GET /api/budgets/current-budgets` | `BudgetResponse[]` |  |
| `GET /api/budget-records/by-budget-ids` | object | 键为预算 ID，值为 `BudgetRecordResponse[]` |
| `POST /api/budget-generation` | 无有效输出 | 方法当前直接 `return null`，待实现 |
| `POST /api/budget-generation/{id}/confirm` | 无有效输出 | 方法当前直接 `return null`，待实现 |

`BudgetResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 预算 ID |
| `transactionCategoryId` | integer(int64) | 收支分类 ID |
| `transactionCategoryName` | string | 收支分类名称 |
| `amount` | number(decimal) | 预算金额 |
| `period` | integer(enum) | `0` 年，`1` 月，`2` 季度 |
| `remaining` | number(decimal) | 剩余预算 |
| `remark` | string/null | 备注 |
| `startTime` | string(date-time) | 开始时间 |
| `endTime` | string(date-time) | 结束时间 |

`BudgetRecordResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `budgetId` | integer(int64) | 预算 ID |
| `recordDate` | string(date-time) | 记录日期 |
| `usedAmount` | number(decimal) | 使用金额；正数消耗，负数回补 |
| `period` | integer(enum) | `0` 年，`1` 月，`2` 季度 |
| `transactionCategoryId` | integer(int64) | 收支分类 ID |

声明但当前未真正输出的 `BudgetGenerationResponse` 字段：`id`、`transactionCategoryId`、`transactionCategoryName`、`amount`、`period`、`remark`、`startTime`、`endTime`。

### 4.4 财务健康

| 方法与路径 | 成功输出 |
| --- | --- |
| `POST /api/financial-health/calculate` | `FinancialHealthScoreResponse` |
| `GET /api/financial-health/score` | `FinancialHealthScoreResponse` 或 `null` |
| `GET /api/financial-health/history` | `PageResponse<FinancialHealthScoreResponse>` |
| `GET /api/financial-health/suggestions` | `FinancialSuggestionResponse[]` |
| `GET /api/financial-health/suggestions/all` | `FinancialSuggestionResponse[]` |
| `GET /api/health` | 字符串 `"Healthy"` |

`FinancialHealthScoreResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 记录 ID |
| `accountBookId` | integer(int64) | 账本 ID |
| `totalScore` | number(decimal) | 总评分 |
| `incomeExpenseRatioScore` | number(decimal) | 收支比率得分 |
| `savingsRateScore` | number(decimal) | 储蓄率得分 |
| `budgetComplianceScore` | number(decimal)/null | 预算执行率得分 |
| `incomeStabilityScore` | number(decimal) | 收入稳定性得分 |
| `healthLevel` | integer | `0` 较差，`1` 一般，`2` 良好，`3` 优秀 |
| `healthLevelName` | string | 健康等级名称 |
| `periodStart` | string(date-time) | 统计周期开始 |
| `periodEnd` | string(date-time) | 统计周期结束 |
| `createDateTime` | string(date-time) | 创建时间 |

`FinancialSuggestionResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `dimension` | string | 评分维度 |
| `score` | number(decimal) | 维度得分 |
| `suggestion` | string | 改善建议 |
| `priority` | string | `High` / `Medium` / `Low` |

### 4.5 支付方式与分类

| 方法与路径 | 成功输出 |
| --- | --- |
| `POST /api/payment-methods` | integer(int64)，新支付方式 ID |
| `DELETE /api/payment-methods/{id}` | boolean |
| `PUT /api/payment-methods/{id}` | boolean |
| `PATCH /api/payment-methods/{id}/default` | boolean |
| `GET /api/payment-methods` | `PaymentMethodResponse[]` |
| `POST /api/transaction-categories` | integer(int64)，新分类 ID |
| `GET /api/transaction-categories/by-parent/{parentId}` | `TransactionCategoryResponse[]` |
| `PUT /api/transaction-categories/{id}` | boolean |
| `PUT /api/transaction-categories/update-parent` | boolean |
| `DELETE /api/transaction-categories/batch` | boolean |

`PaymentMethodResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 支付方式 ID |
| `name` | string | 名称 |
| `type` | integer(enum) | `0` 现金，`1` 信用卡，`2` 电子支付 |
| `electronicPaymentType` | integer(enum)/null | `0` 支付宝，`1` 微信，`2` 银联，`3` 其他 |
| `isDefault` | boolean | 是否默认 |
| `remark` | string/null | 备注 |
| `createDateTime` | string(date-time) | 创建时间 |

`TransactionCategoryResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 分类 ID |
| `name` | string | 分类名称 |
| `type` | integer | `0` 收入，`1` 支出 |

分类新增或更新失败时，控制器返回 HTTP 500，响应体为纯字符串，例如 `"Failed to add category."`。

### 4.6 周期性支出

| 方法与路径 | 成功输出 | 当前实现备注 |
| --- | --- | --- |
| `POST /api/recurring-expense-rule` | integer(int64)，新规则 ID |  |
| `PUT /api/recurring-expense-rule` | integer(int64)，规则 ID |  |
| `POST /api/recurring-expense-rule` | boolean | 与新增接口完全同路由，运行时会产生动作匹配歧义 |
| `GET /api/recurring-expense-rule` | `RecurringExpenseRuleResponse` | `id` 标注为路由参数，但路由模板没有 `{id}` |
| `POST /page` | `PageResponse<RecurringExpenseRuleResponse>` | 属性路由以 `/` 开头，绕过控制器前缀 |

`RecurringExpenseRuleResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 规则 ID |
| `accountBookId` | integer(int64) | 账本 ID |
| `title` | string | 标题 |
| `amount` | number(decimal) | 金额 |
| `categoryId` | integer(int64) | 分类 ID |
| `startDate` | string(date-time) | 开始日期 |
| `endDate` | string(date-time) | 结束日期 |
| `frequency` | integer(enum) | `0` 天，`1` 周，`2` 月，`3` 季度，`4` 年 |
| `currencyId` | integer(int64) | 币种 ID |
| `createUserId` | integer(int64) | 创建人 ID |

### 4.7 分摊支出

| 方法与路径 | 成功输出 |
| --- | --- |
| `POST /api/shared-expenses` | integer(int64)，新分摊项目 ID |
| `GET /api/shared-expenses/{id}` | `SharedExpenseResponse` |
| `PUT /api/shared-expenses/{id}` | boolean |
| `DELETE /api/shared-expenses/{id}` | boolean |
| `GET /api/shared-expenses` | `SharedExpenseResponse[]` |
| `POST /api/shared-expense-reminders` | integer(int64)，新提醒 ID |
| `GET /api/shared-expense-reminders/by-shared-expense/{sharedExpenseId}` | `SharedExpenseReminderResponse[]` |
| `PUT /api/shared-expense-reminders/{id}/status` | boolean |
| `POST /api/shared-expense-settlements` | integer(int64)，新结算 ID |
| `GET /api/shared-expense-settlements/{id}` | `SharedExpenseSettlementResponse` |
| `GET /api/shared-expense-settlements/by-shared-expense/{sharedExpenseId}` | `SharedExpenseSettlementResponse[]` |

`SharedExpenseResponse`

| 字段 | 类型 |
| --- | --- |
| `id` | integer(int64) |
| `totalAmount` | number(decimal) |
| `payerId` | integer(int64) |
| `accountingId` | integer(int64) |
| `accountBookId` | integer(int64) |
| `title` | string |
| `currencyId` | integer(int64) |
| `transactionCategoryId` | integer(int64) |
| `expenseDate` | string(date-time) |
| `splitType` | integer(enum)：`0` 按比例，`1` 人均，`2` 固定金额，`3` 混合 |
| `status` | integer(enum)：`0` 未支付，`1` 部分支付，`2` 全部支付 |
| `description` | string/null |
| `participants` | `SharedExpenseParticipantResponse[]` |

`SharedExpenseParticipantResponse`

| 字段 | 类型 |
| --- | --- |
| `id` | integer(int64) |
| `sharedExpenseId` | integer(int64) |
| `participantId` | integer(int64) |
| `shareAmount` | number(decimal) |
| `shareRatio` | number(decimal)/null |
| `isPaid` | boolean |
| `settlementDate` | string(date-time)/null |
| `accountingId` | integer(int64)/null |
| `proofUrl` | string/null |
| `remark` | string/null |

`SharedExpenseReminderResponse`

| 字段 | 类型 |
| --- | --- |
| `id` | integer(int64) |
| `sharedExpenseId` | integer(int64) |
| `participantId` | integer(int64) |
| `reminderId` | integer(int64) |
| `reminderType` | integer(enum)：`0` 未支付，`1` 逾期，`2` 手动 |
| `status` | integer(enum)：`0` 待发送，`1` 已发送，`2` 失败，`3` 已读，`4` 已处理 |
| `content` | string/null |
| `scheduledTime` | string(date-time) |
| `sentTime` | string(date-time)/null |
| `nextReminderTime` | string(date-time)/null |

`SharedExpenseSettlementResponse`

| 字段 | 类型 |
| --- | --- |
| `id` | integer(int64) |
| `sharedExpenseId` | integer(int64) |
| `participantId` | integer(int64) |
| `receiverId` | integer(int64) |
| `paymentAmount` | number(decimal) |
| `paymentMethod` | string/null |
| `settlementDate` | string(date-time) |
| `accountingId` | integer(int64) |
| `proofUrl` | string/null |
| `remark` | string/null |

## 5. Identity Service

### 5.1 授权与用户

| 方法与路径 | 成功输出 |
| --- | --- |
| `POST /api/auth/token` | OpenIddict OAuth 令牌响应 |
| `POST /api/auth/register` | integer(int64)，新用户 ID |
| `POST /api/auth/emailVerificationCode` | 空 |
| `POST /api/auth/smsVerificationCode` | 空 |
| `POST /api/auth/email/bind` | 空 |
| `POST /api/auth/phone/bind` | 空 |
| `PUT /api/auth/password/reset` | 空 |
| `POST /api/auth/logout` | `{ message, user_id, username }` |
| `POST /api/auth/connect/logout` | OpenIddict `SignOut` 响应 |
| `POST /api/auth/revoke` | `{ message, user_id, username, revoked_at }` |
| `GET /api/auth/userinfo` | `{ sub, name, email, updated_at }` |
| `POST /api/auth/introspect` | RFC 7662 风格令牌状态 |
| `GET /api/users/{id}` | `UserResponse` |
| `GET /api/users` | `PageResponse<UserResponse>` |
| `DELETE /api/users/{id}` | 空 |
| `PUT /api/users/{id}/status` | 空 |
| `PUT /api/users/{id}` | 空 |
| `GET /api/users/phone/verify` | boolean |
| `GET /api/users/email/verify` | boolean |

典型 OAuth 令牌响应由 OpenIddict 生成：

```json
{
  "token_type": "Bearer",
  "access_token": "...",
  "expires_in": 3600,
  "refresh_token": "...",
  "scope": "api offline_access"
}
```

无效令牌的内省输出为 `{ "active": false }`；有效令牌包含 `active`、`sub`、`username`、`email`、`scope`、`client_id`、`token_type`、`iat`、`exp`、`nbf`、`aud`、`iss`、`jti`、`roles`、`permissions`。

`UserResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `id` | integer(int64) | 用户 ID |
| `userName` | string | 用户名 |
| `email` | string | 邮箱 |
| `phoneNumber` | string | 手机号 |
| `isLocked` | boolean | 是否锁定 |

### 5.2 验证码、角色与客户端

| 方法与路径 | 成功输出 |
| --- | --- |
| `GET /api/captcha/create` | `CaptchaCreateResponse` |
| `POST /api/captcha/verify` | `{ "success": boolean }` |
| `GET /api/roles` | `PageResponse<RoleResponse>` |
| `GET /api/roles/{id}` | `RoleResponse` |
| `POST /api/roles` | 空 |
| `PUT /api/roles/{id}` | 空 |
| `DELETE /api/roles/{id}` | 空 |
| `POST /api/roles/{roleId}/permissions` | 空 |
| `GET /api/roles/{roleId}/permissions` | `string[]` |
| `DELETE /api/roles/{roleId}/permissions/{permission}` | 空 |
| `GET /api/roles/users/{userId}/permissions` | `string[]` |
| `POST /api/clients/register` | `{ success, message, client_id }` |
| `DELETE /api/clients/{clientId}` | `{ success, message, client_id }` |
| `GET /api/clients` | `{ clients, count }`；`clients` 元素结构由服务层返回值决定 |
| `GET /api/clients/{clientId}/exists` | `{ client_id, exists }` |
| `POST /api/clients/initialize` | `{ success, message }`，非开发/Local 环境返回 404 空响应 |

`CaptchaCreateResponse`：`token: string`、`imageBase64: string`、`expiresInSeconds: integer`。

`RoleResponse`：`id: integer(int64)`、`roleName: string`。

客户端接口的业务错误结构为 `{ "error": "错误码", "error_description": "说明" }`。

### 5.3 OpenID Connect 元数据

| 方法与路径 | 成功输出 |
| --- | --- |
| `GET /.well-known/openid-configuration` | OIDC 配置对象 |
| `GET /.well-known/openid_configuration` | 同上，兼容旧路径 |
| `GET /.well-known/jwks` | `{ "keys": [] }`；当前使用对称签名，不公开密钥 |
| `GET /.well-known/health` | `{ status, timestamp, service, version }` |

OIDC 配置对象包含：`issuer`、`token_endpoint`、`userinfo_endpoint`、`jwks_uri`、`end_session_endpoint`、`revocation_endpoint`、`introspection_endpoint`、`subject_types_supported`、`id_token_signing_alg_values_supported`、`scopes_supported`、`token_endpoint_auth_methods_supported`、`claims_supported`、`grant_types_supported`。

## 6. ML Service

| 方法与路径 | 成功输出 |
| --- | --- |
| `GET /api/ai-usage-record/{userId}` | `AIUsageRecordResponse[]` |
| `POST /api/ai-usage-record` | boolean |
| `POST /api/CategoryPrediction/predict` | `PredictionResponse` |
| `POST /api/CategoryPrediction/feedback/choice` | `{ "message": "Choice recorded successfully" }` |
| `POST /api/CategoryPrediction/feedback/correction` | `{ "message": "Correction recorded successfully" }` |
| `GET /api/CategoryPrediction/stats` | `LearningStatsResponse` |
| `POST /api/CategoryPrediction/retrain` | `{ "message": "Retraining triggered successfully" }` |

`AIUsageRecordResponse`：`userId: integer(int64)`、`usageType: integer(enum)`；当前枚举值 `1` 表示生成预算。

`PredictionResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `predictedCategory` | object | `{ id: string, name: string }` |
| `confidence` | number(float) | 置信度 0～1 |
| `method` | string | `RuleBased` 或 `MachineLearning` |
| `requiresUserConfirmation` | boolean | 是否需要用户确认 |

`LearningStatsResponse`：`totalFeedbacks: integer`、`trainingDataSize: integer`、`modelExists: boolean`、`recentFeedbacks: integer`。

预测、反馈和重训发生错误时直接返回 HTTP 400：`{ "error": "错误信息" }`。

## 7. Notification Service

| 方法与路径 | 成功输出 | 当前实现备注 |
| --- | --- | --- |
| `POST /api/insite-notifications` | integer(int64)，通知 ID |  |
| `POST /api/insite-notifications/all` | 空 | 声明为 `long`，实际 `Ok()` |
| `PUT /api/insite-notifications/{notificationId}/read` | 空 | 声明为 `bool`，实际 `Ok()` |
| `GET /api/insite-notifications` | `PageResponse<InSiteNotificationResponse>` | 代码类型名为 `InSiteNotificationRequest` |
| `GET /api/insite-notifications/unread-count` | integer |  |
| `DELETE /api/insite-notifications` | 空 | 声明为 `bool`，实际 `Ok()` |
| `PUT /api/insite-notifications` | 空 | 声明为 `bool`，实际 `Ok()` |
| `GET /api/insite-notifications/{notificationId}` | `InSiteNotificationResponse` | 代码类型名为 `InSiteNotificationRequest` |

`InSiteNotificationResponse`

| 字段 | 类型 |
| --- | --- |
| `id` | integer(int64) |
| `title` | string |
| `content` | string |
| `isRead` | boolean |
| `createdAt` | string(date-time) |

## 8. Report Service

| 方法与路径 | 成功输出 |
| --- | --- |
| `GET /api/budget-reports/budget-progress` | `BudgetProgressReportResponse[]` |
| `GET /api/budget-reports/budget-consumption-trend` | `BudgetConsumptionTrendReportResponse[]` |
| `GET /api/budget-reports/budget-insight` | `ReportInsightResponse` |
| `POST /api/Report/GetReport` | `ReportResponse[]` |
| `POST /api/Report/GetReportInsight` | `ReportInsightResponse` |

`BudgetProgressReportResponse`：`period: integer(enum)`、`isComprehensive: boolean`、`categoryName: string`、`totalAmount: decimal`、`usedAmount: decimal`、`remaining: decimal`、`reportDate: string`。

`BudgetConsumptionTrendReportResponse`：`period: integer(enum)`、`isComprehensive: boolean`、`categoryName: string`、`timePoint: string`、`consumedAmount: decimal`。

`ReportResponse`：`year: integer`、`month: integer`、`amount: decimal`。

`ReportInsightResponse`

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `insightType` | string | 解读类型 |
| `dataPeriod` | string | 数据周期 |
| `summary` | string | 概括结论 |
| `healthLevel` | string | `Healthy` / `Attention` / `Risk` / `Overrun` / `NoData` |
| `generatedBy` | string | `Rule` / `LLM` |
| `generatedAt` | string(date-time) | 生成时间 |
| `metrics` | `ReportInsightMetricResponse[]` | 核心指标 |
| `highlights` | `ReportInsightItemResponse[]` | 亮点 |
| `risks` | `ReportInsightItemResponse[]` | 风险提醒 |
| `suggestions` | `ReportInsightItemResponse[]` | 行动建议 |

`ReportInsightMetricResponse`：`name: string`、`value: decimal`、`unit: string`、`description: string`。

`ReportInsightItemResponse`：`dimension: string`、`title: string`、`content: string`、`severity: string`（`Info` / `Low` / `Medium` / `High`）。

仓库中还定义了但当前控制器未直接输出的报表模型：

- `BudgetActualComparisonReportResponse`：`period`、`isComprehensive`、`categoryName`、`budgetedAmount`、`actualAmount`、`variance`。
- `HistoricalBudgetComparisonReportResponse`：`period`、`isComprehensive`、`categoryName`、`dateTime`、`budgetedAmount`。

## 9. Resource Service

| 方法与路径 | 成功输出 | 当前实现备注 |
| --- | --- | --- |
| `POST /api/assistant/extract-amount-and-category` | `AmountAndCategoryExtractionResponse` |  |
| `POST /api/files/upload` | 空 |  |
| `GET /api/files/url` | string | JSON 字符串形式的 URL |
| `DELETE /api/files` | 空 | action 返回 `Task`，无显式结果 |
| `GET /api/files/upload-token` | `PresignedURLResponse` | 见下方序列化风险 |
| `POST /api/files/confirm-upload` | integer(int64)，文件 ID |  |
| `GET /api/ocr/recognize` | 空 | 触发 OCR 识别 |
| `GET /api/ocr/text` | string 或 `null` | 已识别文字 |

`AmountAndCategoryExtractionResponse`

| 字段 | 类型 |
| --- | --- |
| `category` | string/null |
| `amount` | number(decimal)/null |
| `currency` | string/null |

`PresignedURLResponse` 设计字段为 `uploadUrl`、`objectName`、`originalFileName`。但这三个成员当前是公共字段而非属性，且项目中未发现 `JsonSerializerOptions.IncludeFields = true`；按 System.Text.Json 默认行为，实际响应可能是 `{}`。建议改为 `{ get; set; }` 属性后再依赖该结构。

代码中定义了 `OcrTextResponse`（`text: string`、`lines: string[]`），但当前公开 OCR 控制器没有直接返回该模型。

## 10. Gateway

| 方法与路径 | 成功输出 | 错误输出 |
| --- | --- | --- |
| `GET /openapi/proxy/{serviceName}` | 下游服务的原始 OpenAPI JSON | 404 空；503/502/下游错误状态均为纯字符串说明 |

## 11. 当前输出层面的已知问题

1. 多个接口的泛型声明与实际响应体不一致（声明 `bool`/`long`，实际为空），客户端不应仅依赖方法签名推断 JSON。
2. `BudgetGenerationController` 两个接口尚未实现，当前返回 `null`。
3. `RecurringExpenseRuleController` 的新增与删除动作共享 `POST /api/recurring-expense-rule`，可能触发 `AmbiguousMatchException`；分页路由实际为根路径 `/page`。
4. `PresignedURLResponse` 使用字段而非属性，默认 JSON 序列化可能输出空对象。
5. 成功响应没有统一包装，错误响应同时存在 `ExceptionResponse`、OAuth 风格对象、匿名 `{ error }` 和纯字符串四种形式。
6. 部分路由由 `[controller]` 生成并保留控制器名称，例如 `/api/CategoryPrediction/...`、`/api/Report/...`；部署环境是否区分大小写取决于网关和宿主。
