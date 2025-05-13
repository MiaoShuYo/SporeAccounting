namespace SP.Common.Logger;

/// <summary>
/// Loki日志配置服务接口
/// </summary>
public interface ILokiLoggerConfigService
{
    /// <summary>
    /// 配置并返回Serilog日志记录器
    /// </summary>
    /// <returns>已配置的Serilog日志记录器</returns>
    Serilog.Core.Logger ConfigureLogger();
}