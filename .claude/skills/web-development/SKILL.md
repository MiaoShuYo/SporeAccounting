---
name: web-development
description: 开发、调试或评审 SporeAccounting 的 Vue 3 Web 客户端，包括页面、路由、Element Plus 组件、Axios 调用和前端构建；仅在任务涉及 web/ 时使用。
---

# SporeAccounting Web 开发

Web 工程位于 `web/`，技术栈为 Vue 3、TypeScript、Vite、Vue Router、Element Plus、Axios 和 ECharts。

## 项目事实

- 入口是 `web/src/main.ts`，全局 Axios Base URL 读取 `VITE_API_BASE_URL`，缺省回退到 `/api`。
- 路由位于 `web/src/router/index.ts`，当前使用 memory history；不要在未确认部署方式前改成 history/hash 模式。
- 页面位于 `web/src/pages/`，共享类型位于 `web/src/Interface/`。
- Token 当前存于 localStorage，401 会清理会话并跳转登录页。涉及认证时先理解现有拦截器和路由守卫，避免重复跳转或刷新风暴。
- 部分旧页面仍使用历史接口路径和 `Response<T>` 包装，但新后端实际响应通常无统一外层。新增或修复请求时以 Controller、Gateway 与 `api/docs/api-response-reference.md` 为准，不复制旧调用中的错误假设。

## 实施约定

1. 修改页面前检查对应路由、类型文件、后端 Controller 和响应文档，先确定真实方法、路径、请求体与响应形状。
2. 使用 `<script setup lang="ts">` 和明确类型；避免继续扩散 `any`。接口 DTO 名称和 camelCase 字段应与实际 JSON 对齐。
3. 复用 Element Plus 的表单校验、加载态、空态、错误提示和确认对话框；删除等破坏性操作必须二次确认。
4. 所有业务请求经 API Gateway。Authorization 使用 `Bearer <token>`；不要伪造网关内部请求头。
5. 金额展示与计算避免二进制浮点累积误差；时间展示明确时区；服务端整数枚举提供未知值兜底。
6. 数据页至少处理 loading、empty、error、content；分页还需处理加载中、失败重试和没有更多数据；提交期间禁止重复点击。
7. 环境差异通过 Vite 环境变量处理。不要把真实密码、Token 或客户端 Secret 写进前端包。
8. 只改与任务相关的页面和接口类型；若后端契约确有问题，先说明跨端影响，不在未授权时顺带改后端。

## 验证

- 依赖已存在时运行 `npm run build --prefix web`。
- 需要干净安装时运行 `npm ci --prefix web`，不要提交 `node_modules/` 或 `dist/`。
- 修改交互后检查正常、空数据、请求失败、401、表单校验和窄屏布局。
- 不主动推送或部署生产站点，除非用户明确要求。
