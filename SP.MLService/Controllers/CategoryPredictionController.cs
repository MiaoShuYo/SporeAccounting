using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Common;
using SP.MLService.Domain;
using SP.MLService.Models;
using SP.MLService.Models.Dto;
using SP.MLService.Models.Request;
using SP.MLService.Models.Response;
using SP.MLService.Services;

namespace SP.MLService.Controllers
{
    /// <summary>
    /// 消费类目预测控制器
    /// 提供基于机器学习的消费类目智能预测与渐进式学习功能
    /// </summary>
    /// <remarks>
    /// 主要功能：
    /// 1. 智能预测消费类目（结合规则匹配与ML模型）
    /// 2. 记录用户反馈（正确选择和错误纠正）
    /// 3. 触发增量学习优化模型
    /// 4. 提供学习统计信息
    /// </remarks>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CategoryPredictionController : ControllerBase
    {
        /// <summary>
        /// 渐进式学习管理器，负责模型训练、预测和反馈收集
        /// </summary>
        private readonly ProgressiveLearningManager _learningManager;

        /// <summary>
        /// 当前请求的用户会话（从网关转发的 X-User-Id 等请求头中提取）
        /// </summary>
        private readonly ContextSession _contextSession;

        /// <summary>
        /// 构造函数，通过依赖注入获取渐进式学习管理器和用户会话
        /// </summary>
        /// <param name="learningManager">渐进式学习管理器实例</param>
        /// <param name="contextSession">当前请求用户会话</param>
        public CategoryPredictionController(ProgressiveLearningManager learningManager, ContextSession contextSession)
        {
            _learningManager = learningManager;
            _contextSession = contextSession;
        }

        /// <summary>
        /// 智能预测消费类目
        /// </summary>
        /// <param name="request">预测请求，包含查询文本和候选类目列表</param>
        /// <returns>预测结果，包含推荐类目、置信度、预测方法等信息</returns>
        /// <remarks>
        /// 预测流程：
        /// 1. 如果模型未训练或数据不足，使用规则匹配（关键词）
        /// 2. 如果模型已训练，使用机器学习预测
        /// 3. 根据置信度阈值判断是否需要用户确认
        /// 4. 预测失败时自动回退到规则匹配
        /// </remarks>
        [HttpPost("predict")]
        public async Task<ActionResult<PredictionResponse>> Predict([FromBody] PredictionRequest request)
        {
            try
            {
                // 将DTO转换为领域对象
                var categories = request.Categories.Select(c => new UserCategory(c.Id, c.Name)).ToList();

                // 调用智能预测（自动选择规则匹配或ML预测）
                var prediction = _learningManager.SmartPredict(
                    request.Query, // 查询文本（如"星巴克咖啡"）
                    categories, // 用户的全部类目列表
                    _contextSession.UserId.ToString(), // 用户ID（从 JWT/网关转发的请求头中获取）
                    request.Merchant, // 商户信息（可选）
                    request.AmountBucket, // 金额分箱（0-4）
                    request.HourOfDay // 消费时间（0-23小时）
                );

                // 构造响应结果
                return Ok(new PredictionResponse
                {
                    PredictedCategory = new CategoryDto
                    {
                        Id = prediction.PredictedCategory.Id,
                        Name = prediction.PredictedCategory.Name
                    },
                    Confidence = prediction.Confidence, // 置信度分数（0-1）
                    Method = prediction.Method.ToString(), // 预测方法：RuleBased 或 MachineLearning
                    RequiresUserConfirmation = prediction.RequiresUserConfirmation // 是否需要用户确认
                });
            }
            catch (Exception ex)
            {
                // 异常处理：返回错误信息
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 记录用户正确选择（正反馈）
        /// </summary>
        /// <param name="request">用户选择反馈请求</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 用于记录用户主动选择某个类目的行为，作为正样本用于模型训练。
        /// 系统会根据配置的重训频率自动触发增量学习。
        /// </remarks>
        [HttpPost("feedback/choice")]
        public async Task<ActionResult> RecordChoice([FromBody] ChoiceFeedbackRequest request)
        {
            try
            {
                // 转换DTO为领域对象
                var selectedCategory = new UserCategory(request.SelectedCategory.Id, request.SelectedCategory.Name);
                var availableCategories = request.AvailableCategories.Select(c => new UserCategory(c.Id, c.Name));

                // 记录用户选择，将作为正样本加入训练数据
                _learningManager.RecordUserChoice(
                    request.Query, // 原始查询文本
                    selectedCategory, // 用户选择的类目（正样本）
                    availableCategories, // 当时可选的全部类目（用于生成负样本）
                    _contextSession.UserId.ToString(), // 用户标识（从 JWT/网关转发的请求头中获取）
                    request.Merchant, // 商户信息
                    request.AmountBucket, // 金额分箱
                    request.HourOfDay // 消费时间
                );

                return Ok(new { message = "Choice recorded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 记录用户纠正（负反馈+正反馈）
        /// </summary>
        /// <param name="request">用户纠正反馈请求</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 用于记录用户纠正模型预测错误的行为：
        /// - 错误预测类目作为负样本
        /// - 用户纠正的正确类目作为正样本
        /// 纠正反馈比普通选择更重要，会立即触发增量学习
        /// </remarks>
        [HttpPost("feedback/correction")]
        public async Task<ActionResult> RecordCorrection([FromBody] CorrectionFeedbackRequest request)
        {
            try
            {
                // 转换DTO为领域对象
                var wrongCategory = new UserCategory(request.WrongCategory.Id, request.WrongCategory.Name);
                var correctCategory = new UserCategory(request.CorrectCategory.Id, request.CorrectCategory.Name);
                var availableCategories = request.AvailableCategories.Select(c => new UserCategory(c.Id, c.Name));

                // 记录用户纠正，立即触发模型重训
                _learningManager.RecordUserCorrection(
                    request.Query, // 原始查询文本
                    wrongCategory, // 模型错误预测的类目（负样本）
                    correctCategory, // 用户纠正的正确类目（正样本）
                    availableCategories, // 当时可选的全部类目
                    _contextSession.UserId.ToString(), // 用户标识（从 JWT/网关转发的请求头中获取）
                    request.Merchant, // 商户信息
                    request.AmountBucket, // 金额分箱
                    request.HourOfDay // 消费时间
                );

                return Ok(new { message = "Correction recorded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 获取学习统计信息
        /// </summary>
        /// <returns>包含反馈数量、训练数据规模、模型状态等统计信息</returns>
        /// <remarks>
        /// 用于监控渐进式学习的进展：
        /// - 总反馈数：用户提供的选择和纠正总数
        /// - 训练数据量：转换为LTR格式的训练样本数
        /// - 模型状态：是否存在已训练的模型文件
        /// - 近期反馈：最近的反馈数量
        /// </remarks>
        [HttpGet("stats")]
        public async Task<ActionResult<LearningStatsResponse>> GetStats()
        {
            // 获取学习管理器的统计信息
            var stats = _learningManager.GetStats();

            // 构造响应对象
            return Ok(new LearningStatsResponse
            {
                TotalFeedbacks = stats.TotalFeedbacks, // 总反馈数
                TrainingDataSize = stats.TrainingDataSize, // 训练数据量
                ModelExists = stats.ModelExists, // 模型是否存在
                RecentFeedbacks = stats.RecentFeedbacks // 近期反馈数
            });
        }

        /// <summary>
        /// 手动触发模型重训
        /// </summary>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 管理员功能：手动强制触发增量学习，无需等待自动重训条件。
        /// 适用场景：
        /// - 收集到大量新反馈后立即优化模型
        /// - 系统维护时的模型更新
        /// - 测试和调试场景
        /// </remarks>
        [HttpPost("retrain")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> TriggerRetraining()
        {
            try
            {
                // 手动触发增量学习
                _learningManager.TriggerIncrementalLearning();
                return Ok(new { message = "Retraining triggered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}