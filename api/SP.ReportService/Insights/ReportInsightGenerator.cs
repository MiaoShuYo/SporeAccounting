using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SP.Common.LLM;
using SP.ReportService.Models.Response;

namespace SP.ReportService.Insights;

/// <summary>
/// 报表智能解读生成器
/// </summary>
public class ReportInsightGenerator
{
    private readonly ReportInsightRuleEngine _ruleEngine;
    private readonly ReportInsightOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReportInsightGenerator> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ReportInsightGenerator(
        ReportInsightRuleEngine ruleEngine,
        IOptions<ReportInsightOptions> options,
        IServiceProvider serviceProvider,
        ILogger<ReportInsightGenerator> logger)
    {
        _ruleEngine = ruleEngine;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 生成智能解读
    /// </summary>
    public async Task<ReportInsightResponse> GenerateAsync(
        ReportInsightPortrait portrait,
        CancellationToken cancellationToken = default)
    {
        ReportInsightResponse ruleResult = _ruleEngine.Generate(portrait);

        if (!_options.EnableLlm)
        {
            return ruleResult;
        }

        var openAIService = _serviceProvider.GetService<IOpenAIService>();
        if (openAIService == null)
        {
            _logger.LogWarning("[报表解读] 已启用 LLM，但未注册 OpenAI 服务，使用规则引擎结果");
            return ruleResult;
        }

        try
        {
            string prompt = ReportInsightPromptBuilder.Build(portrait, ruleResult);
            var llmResult = await openAIService.ChatStructuredAsync<ReportInsightResponse>(prompt, cancellationToken);

            if (llmResult == null || string.IsNullOrWhiteSpace(llmResult.Summary))
            {
                _logger.LogWarning("[报表解读] LLM 返回空结果，使用规则引擎结果");
                return ruleResult;
            }

            llmResult.InsightType = string.IsNullOrWhiteSpace(llmResult.InsightType)
                ? ruleResult.InsightType
                : llmResult.InsightType;
            llmResult.DataPeriod = string.IsNullOrWhiteSpace(llmResult.DataPeriod)
                ? ruleResult.DataPeriod
                : llmResult.DataPeriod;
            llmResult.HealthLevel = string.IsNullOrWhiteSpace(llmResult.HealthLevel)
                ? ruleResult.HealthLevel
                : llmResult.HealthLevel;
            llmResult.GeneratedBy = "LLM";
            llmResult.GeneratedAt = DateTime.Now;

            if (llmResult.Metrics.Count == 0)
            {
                llmResult.Metrics = ruleResult.Metrics;
            }

            return llmResult;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[报表解读] LLM 生成失败，使用规则引擎结果");
            return ruleResult;
        }
    }
}