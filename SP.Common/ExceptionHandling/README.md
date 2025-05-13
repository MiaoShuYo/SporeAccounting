# 异常处理中间件

此目录包含用于全局异常处理的中间件和相关类。

## 功能特性

- 全局捕获未处理的异常
- 记录异常详情到日志系统
- 将详细异常信息记录到Loki日志系统
- 返回统一格式的错误响应
- 支持自定义异常类型和状态码
- 开发环境下提供详细的错误信息（包括堆栈跟踪）
- 生产环境下提供友好的错误消息
- 支持请求体缓冲，使请求体可以被多次读取（对于详细的异常日志记录很有用）

## 如何使用

### 1. 配置Loki日志

在 `appsettings.json` 中添加Loki配置:

```json
{
  "Loki": {
    "Url": "http://loki:3100",
    "AppName": "SporeAccounting",
    "Environment": "development",
    "Username": "optional_username",
    "Password": "optional_password"
  }
}
```

### 2. 在应用程序启动时注册服务和中间件

在 `Program.cs` 或 `Startup.cs` 中添加以下代码：

```csharp
using SP.Common.ExceptionHandling;
using SP.Common.Logger;

// 在ConfigureServices方法中添加日志服务
services.AddLoggerService(Configuration);

// 在Configure方法中添加中间件
// 方法1：分别添加请求缓冲和异常处理中间件
app.UseRequestBuffering(); // 启用请求缓冲
app.UseExceptionHandling(); // 添加异常处理

// 方法2：使用集成的扩展方法（推荐）
app.UseFullExceptionHandling(); // 同时添加请求缓冲和异常处理

// 其他中间件注册...
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// ...
```

> **重要提示**：请确保将这些中间件添加在请求管道的最前面，以确保能够捕获所有异常。

### 3. 抛出自定义异常

在业务代码中，可以抛出自定义异常，中间件会自动处理并返回相应的状态码和消息：

```csharp
using SP.Common.ExceptionHandling.Exceptions;

// 资源未找到异常（返回404）
throw new NotFoundException("用户", userId);

// 错误请求异常（返回400）
throw new BadRequestException("无效的请求参数");

// 未授权异常（返回401）
throw new UnauthorizedException();

// 禁止访问异常（返回403）
throw new ForbiddenException("您没有权限访问此资源");

// 验证异常（返回400并包含详细的验证错误）
var errors = new Dictionary<string, string[]>
{
    { "Username", new[] { "用户名已存在" } },
    { "Email", new[] { "邮箱格式不正确" } }
};
throw new ValidationException("提交的数据验证失败", errors);

// 自定义异常（可自定义状态码）
throw new AppException("自定义错误消息", HttpStatusCode.ServiceUnavailable);
```

### 4. 使用日志服务

可以在任何需要记录日志的地方注入并使用日志服务：

```csharp
using SP.Common.Logger;

public class MyService
{
    private readonly ILoggerService _loggerService;

    public MyService(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }

    public void DoSomething()
    {
        try
        {
            // 业务逻辑...
            _loggerService.LogInformation("执行了某个操作");
        }
        catch (Exception ex)
        {
            _loggerService.LogError(ex, "发生错误");
            throw;
        }
    }
}
```

### 5. 错误响应格式

中间件返回的错误响应格式如下：

```json
{
  "statusCode": 400,
  "errorMessage": "提交的数据验证失败",
  "stackTrace": "..." // 仅在开发环境中显示
}
```

对于验证异常，还会包含详细的验证错误信息：

```json
{
  "statusCode": 400,
  "errorMessage": "提交的数据验证失败",
  "errors": {
    "Username": ["用户名已存在"],
    "Email": ["邮箱格式不正确"]
  }
}
```

### 6. Loki日志记录

异常处理中间件会自动将详细的异常信息记录到Loki日志系统中，包括以下信息：

- 请求详情（URL、HTTP方法、查询参数、请求头、请求体）
- 异常详情（消息、类型、堆栈跟踪、内部异常）
- 用户信息（用户ID、认证状态、IP地址）
- 时间戳

在Grafana中，你可以使用类似以下的查询语句查看异常日志：

```
{app="SporeAccounting"} |= "处理请求发生异常"
```

可以使用Grafana的JSON解析功能进一步筛选和分析异常数据：

```
{app="SporeAccounting"} |= "处理请求发生异常" | json | exceptionInfo_exceptionType =~ ".*NotFoundException.*"
``` 