---
name: api-development
description: 开发、调试或评审 SporeAccounting 的 ASP.NET Core 微服务、网关、数据库迁移和后端 API；仅在任务涉及 api/ 时使用。
---

# SporeAccounting API 开发

在仓库根目录工作，后端代码位于 `api/`，解决方案入口为 `api/SporeAccounting.sln`。

## 项目基线

- 目标框架为 .NET 10；不要擅自降级 TargetFramework 或混用其他 .NET 主版本。
- `SP.Gateway` 是客户端唯一业务入口。Web 和 Android 不应绕过网关直接访问微服务。
- `SP.Common` 承载异常处理、会话、日志、Redis、消息、服务发现、Refit 和公共模型。改动它时评估全部微服务影响。
- 数据服务主要使用 EF Core + MySQL；身份认证使用 OpenIddict；服务间调用主要使用 Refit；异步任务使用 Quartz/RabbitMQ。
- 当前成功响应直接返回对象、数组、分页对象或空 body，没有统一 `code/data/message` 外层。不要凭旧客户端代码臆造响应包装。

## 服务定位

- `SP.IdentityService`：身份、授权、用户、角色、验证码与 Token。
- `SP.FinanceService`：账本、记账、预算、支付方式、共享支出与财务健康。
- `SP.ConfigService`：用户和系统配置。
- `SP.CurrencyService`：币种与汇率。
- `SP.ReportService`：报表、预算分析与智能解读。
- `SP.ResourceService`：文件、对象存储、OCR 与助手能力。
- `SP.MLService`：分类预测与学习反馈。
- `SP.NotificationService`：站内通知。
- `SP.Gateway`：鉴权、签名、路由和服务发现。

## 实施约定

1. 先定位所属服务，阅读同一功能的 Controller、Request/Response、Service 接口与 `Impl`、Entity、DbContext 和 AutoMapper 配置。
2. HTTP 路由以 Controller 实现和 Gateway 路由为准；修改契约时同步检查 `api/docs/api-response-reference.md`、`api/docs/android-api-guide.md`、Web 调用和 Android 需求。
3. 保持现有分层：Controller 负责 HTTP 绑定与轻量校验，业务规则放 Service，实现使用项目已有异常类型交给全局中间件处理。
4. 创建或修改实体时使用 `SettingCommProperty` 维护创建、修改和软删除字段；查询默认排除 `IsDeleted` 数据。
5. 金额使用 `decimal`，时间明确 UTC/本地语义，枚举兼容当前整数序列化；不要把密钥或真实环境地址新增到源码。
6. 数据库结构变化必须生成对应服务的 EF Core migration，并检查 model snapshot；不要手写伪迁移替代工具输出。
7. 公共能力优先复用 `SP.Common`，但不要为单一服务的局部逻辑扩大公共层。

## 验证

- 最低验证：`dotnet build api/SporeAccounting.sln`。
- 有测试项目时运行受影响测试；没有测试覆盖时，至少编译受影响项目并静态核对 Controller 路由和响应类型。
- EF 变更额外验证 migration 可创建/应用；Docker 变更从 `api/` 作为构建上下文验证。
- 不主动推送、发布、执行生产迁移或触发部署，除非用户明确要求。
