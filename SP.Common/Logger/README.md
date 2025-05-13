# 日志服务

此目录包含用于日志记录的服务和相关类，支持Grafana Loki作为日志存储后端。

## 功能特性

- 集成Serilog日志框架
- 支持多种日志级别（Debug、Information、Warning、Error、Critical）
- 将日志输出到控制台和Grafana Loki
- 支持结构化日志记录
- 提供简单易用的日志服务接口
- 自动添加应用名称、环境、机器名称等标签
- 提供日志测试工具

## 如何使用

### 1. 配置Loki

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

### 2. 注册日志服务

在 `Program.cs` 或 `Startup.cs` 的 `ConfigureServices` 方法中添加：

```csharp
using SP.Common.Logger;

// ...

services.AddLoggerService(Configuration);
```

### 3. 使用日志服务

在需要记录日志的类中注入并使用日志服务：

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
        // 记录各级别的日志
        _loggerService.LogDebug("这是一条调试日志");
        _loggerService.LogInformation("执行了操作：{OperationName}", "DoSomething");
        _loggerService.LogWarning("这是一条警告日志，参数可能不合适：{Parameter}", "someValue");
        
        try
        {
            // 一些可能会抛出异常的代码...
        }
        catch (Exception ex)
        {
            _loggerService.LogError(ex, "执行操作时发生错误");
            throw;
        }
    }
}
```

### 4. 测试日志连接

可以使用提供的测试工具类验证Loki连接：

```csharp
using SP.Common.Logger;

// 在应用启动时或测试代码中
public void TestLogging(ILoggerService loggerService, IConfiguration configuration)
{
    // 打印Loki配置信息
    LoggingTestTool.PrintLokiConfig(configuration);
    
    // 发送测试日志
    bool testResult = LoggingTestTool.TestLogging(loggerService);
    
    if (testResult)
    {
        Console.WriteLine("日志测试成功！请在Grafana中检查日志。");
    }
    else
    {
        Console.WriteLine("日志测试失败，请检查Loki配置。");
    }
}
```

### 5. 在Grafana中查询日志

在Grafana中添加Loki数据源后，可以使用以下查询来查看日志：

```
{app="SporeAccounting"}
```

也可以使用标签和内容进行过滤：

```
{app="SporeAccounting", environment="development"} |= "错误"
```

或者按日志级别过滤：

```
{app="SporeAccounting"} | level="error"
```

## 日志最佳实践

1. **使用正确的日志级别**:
   - DEBUG: 开发和调试时使用，详细信息
   - INFO: 常规操作信息，如请求开始、完成等
   - WARNING: 潜在问题，但不影响程序运行
   - ERROR: 错误，影响操作但应用可以继续运行
   - CRITICAL: 严重错误，可能导致应用崩溃

2. **使用结构化日志**:
   ```csharp
   // 不要这样做
   _loggerService.LogInformation("用户" + userId + "执行了操作" + operation);
   
   // 正确做法
   _loggerService.LogInformation("用户{UserId}执行了操作{Operation}", userId, operation);
   ```

3. **记录上下文信息**:
   - 包含用户ID、请求ID、相关实体ID等
   - 记录操作相关的参数值
   - 异常日志要包含完整异常信息 