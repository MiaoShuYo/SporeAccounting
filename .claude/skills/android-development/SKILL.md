---
name: android-development
description: 初始化、开发、调试或评审 SporeAccounting Android 客户端，覆盖 Kotlin、Jetpack Compose、网络层、会话、离线缓存和各业务迭代；仅在任务涉及 android/ 时使用。
---

# SporeAccounting Android 开发

Android 目录为 `android/`。开始任何功能前完整阅读 `android/SporeAccountingAndroid-需求规格说明.md` 中相关迭代、Definition of Ready 和 Definition of Done；该文档是当前客户端范围与验收标准的主来源。

## 技术基线

- Kotlin、Jetpack Compose、Material 3、单 Activity、Navigation Compose。
- 架构采用 MVVM 或 MVI，并在工程初始化时选定一种后保持一致。
- Coroutines + Flow；Retrofit + OkHttp；Hilt；Coil；DataStore，结构化缓存需要时使用 Room。
- JSON 在 Gson、Moshi、Kotlin Serialization 中选一种，全工程统一；已有工程时遵循既有选择。
- 最低版本建议 API 26+，最终值由工程初始化决定；开发、测试、生产使用独立 Build Variant 和 `BuildConfig.API_BASE_URL`。

## 不可破坏的约束

- 所有业务请求必须通过 API Gateway；生产环境只允许 HTTPS。
- 匿名接口以外发送 `Authorization: Bearer <access_token>`；禁止客户端伪造 `X-User-*` 或 `X-Gateway-Signature`。
- access token、refresh token、过期时间和必要用户标识使用 DataStore；401 刷新必须并发单飞，失败时原子清理会话并回到登录页。
- 金额使用 `BigDecimal`；DateTime 使用 ISO 8601 并明确 UTC 与本地转换；服务端整数枚举必须显式映射并提供未知值。
- HTTP 200 空 body 映射为 `Unit`，200 + `null` 映射为业务空状态；不要当作 JSON 解析失败。
- Debug 日志必须脱敏 Token、密码、Cookie、客户端密钥和文件预签名 URL。

## 实施方式

1. 先确认当前迭代及前置依赖。后端路由尚未发布或需求文档列为阻塞项时，不用假数据掩盖阻塞；清楚标记并保持可替换边界。
2. 推荐 `core-network`、`core-model`、`core-ui`、`feature-*` 或等价分层。网络 DTO、领域模型和 Compose UI 状态不要混在同一层。
3. Repository 负责数据来源与错误映射，ViewModel 负责状态和事件，Composable 只渲染状态并上报用户意图；UI 层不直接处理 Retrofit `Response`。
4. 每个数据页面覆盖 Loading、Empty、Error、Content；分页覆盖刷新、追加、失败重试和结束状态；提交操作防重复，破坏性操作二次确认。
5. 接口契约同时核对 `api/docs/android-api-guide.md`、`api/docs/api-response-reference.md` 和后端 Controller。文档与代码冲突时说明差异并以可验证的实际网关契约为准。
6. 按需求文档迭代顺序交付，避免在基础网络、会话和通用状态未完成时堆叠业务页面。

## 验证

- 工程存在 Gradle Wrapper 后，Windows 上至少运行 `android/gradlew.bat assembleDebug`、`android/gradlew.bat lint` 和 `android/gradlew.bat test`；按实际模块补充目标任务。
- 网络层必须覆盖 DTO 解析、错误映射、空响应和并发 Token 刷新测试。
- Compose 关键流程覆盖 UI 测试，并验证小屏、深色模式、字体缩放、旋转与进程重建。
- 不主动签名、上传应用商店、发布生产包或写入真实凭据，除非用户明确要求。
