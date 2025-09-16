using System.Text.Json;
using Baidu.Aip.Ocr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Model;
using SP.ResourceService.DB;
using SP.ResourceService.Models.Config;
using SP.ResourceService.Models.Entity;
using SP.ResourceService.Service;

namespace SP.ResourceService.Mq;

/// <summary>
/// 详细队列OCR消费之服务
/// </summary>
public class OCRConsumerService : BackgroundService
{
    /// <summary>
    /// RabbitMq 消息
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<OCRConsumerService> _logger;

    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly ResourceServiceDbContext _dbContext;

    /// <summary>
    /// OSS 服务
    /// </summary>
    private readonly IOssService _ossService;

    /// <summary>
    /// 百度OCR客户端
    /// </summary>
    private readonly Ocr _client;

    /// <summary>
    /// OCR 消费者服务构造函数
    /// </summary>
    /// <param name="options"></param>
    /// <param name="rabbitMqMessage"></param>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    public OCRConsumerService(IOptions<BaiduOCROptions> options, RabbitMqMessage rabbitMqMessage,
        ILogger<OCRConsumerService> logger, ResourceServiceDbContext dbContext)
    {
        _logger = logger;
        _rabbitMqMessage = rabbitMqMessage;
        _dbContext = dbContext;

        try
        {
            // 验证配置
            ValidateConfiguration(options.Value);
            _client = new Ocr(options.Value.APIKey, options.Value.SecretKey);
            // 修改超时时间为60秒
            _client.Timeout = 60000;
            _logger.LogInformation("百度OCR客户端构建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "构建客户端失败");
            throw;
        }
    }

    /// <summary>
    /// 执行异步操作
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqSubscriber subscriber = new MqSubscriber(MqExchange.MessageExchange,
            MqRoutingKey.OCRRoutingKey, MqQueue.OCRQueue);
        await _rabbitMqMessage.ReceiveAsync(subscriber, async message =>
        {
            MqMessage mqMessage = message as MqMessage;

            string body = mqMessage.Body;
            _logger.LogInformation($"接收到OCR消息，消息内容：{body}");
            Files? fileInfo = JsonSerializer.Deserialize<Files>(body);
            if (fileInfo == null)
            {
                _logger.LogError("消息内容转换失败，消息内容为空");
                return;
            }

            // 校验图片是否存在
            Files? file = await _dbContext.Files.FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == fileInfo.Id,
                cancellationToken: stoppingToken);
            if (file == null)
            {
                _logger.LogError("文件不存在，文件id：" + fileInfo.Id);
                return;
            }

            // 从MinIO获取图片
            string url = await _ossService.GetUrlAsync(file.Id);
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogError("获取图片URL失败，文件id：" + fileInfo.Id + "，文件名：" + fileInfo.ObjectName);
                return;
            }

            try
            {
                var image = File.ReadAllBytes("图片文件路径");
                // 如果有可选参数
                var options = new Dictionary<string, object>
                {
                    { "detect_direction", "true" },
                    { "probability", "false" }
                };
                // 带参数调用通用文字识别（高精度版）
                var result = _client.AccurateBasic(image, options);
                if (result == null)
                {
                    _logger.LogError("OCR识别失败，文件id：" + fileInfo.Id);
                    return;
                }

                _logger.LogInformation("OCR识别结果：" + result);
                var words = result["words"];
                if (words == null)
                {
                    _logger.LogError("OCR识别结果为空，文件id：" + fileInfo.Id);
                    return;
                }

                ImageText imageText = new ImageText
                {
                    FileId = fileInfo.Id,
                    RecognizedText = words.ToString()
                };
                SettingCommProperty.Create(imageText);
                await _dbContext.ImageTexts.AddAsync(imageText, stoppingToken);
                await _dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("OCR识别成功，文件id：" + fileInfo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OCR识别失败，文件id：" + fileInfo.Id);
            }
            finally
            {
                await Task.CompletedTask;
            }
        });
    }

    /// <summary>
    /// 验证参数
    /// </summary>
    /// <param name="optionsValue">百度OCR参数</param>
    private void ValidateConfiguration(BaiduOCROptions optionsValue)
    {
        if (string.IsNullOrWhiteSpace(optionsValue.AppId))
        {
            throw new ArgumentException("AppId不能为空");
        }

        if (string.IsNullOrWhiteSpace(optionsValue.APIKey))
        {
            throw new ArgumentException("APIKey不能为空");
        }

        if (string.IsNullOrWhiteSpace(optionsValue.SecretKey))
        {
            throw new ArgumentException("SecretKey不能为空");
        }
    }
}