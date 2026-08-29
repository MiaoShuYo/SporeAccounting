using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Model;
using SP.ResourceService.DB;
using SP.ResourceService.Models.Entity;
using SP.ResourceService.Service;
using SP.ResourceService.Service.OCR;

namespace SP.ResourceService.Mq;

/// <summary>
/// 消息队列OCR消费者服务
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
    /// 作用域工厂
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// OCR 消费者服务构造函数
    /// </summary>
    /// <param name="rabbitMqMessage"></param>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    public OCRConsumerService(RabbitMqMessage rabbitMqMessage,
        ILogger<OCRConsumerService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _rabbitMqMessage = rabbitMqMessage;
        _serviceScopeFactory = serviceScopeFactory;
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
            long fileId = 0L;
            try
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

                fileId = fileInfo.Id;
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ResourceServiceDbContext>();
                var ossService = scope.ServiceProvider.GetRequiredService<IOssService>();
                var ocrProvider = scope.ServiceProvider.GetRequiredService<IOcrProvider>();

                // 校验图片是否存在
                Files? file = await dbContext.Files.FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == fileInfo.Id,
                    cancellationToken: stoppingToken);
                if (file == null)
                {
                    _logger.LogError("文件不存在，文件id：" + fileInfo.Id);
                    return;
                }

                // 从MinIO下载图片
                byte[] image;
                try
                {
                    using var stream = await ossService.DownloadAsync(file.ObjectName, file.IsPublic, stoppingToken);
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream, stoppingToken);
                    image = memoryStream.ToArray();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "下载图片失败，文件id：{FileId}，文件名：{ObjectName}", fileInfo.Id, fileInfo.ObjectName);
                    return;
                }

                var result = await ocrProvider.RecognizeAsync(image, stoppingToken);
                if (string.IsNullOrWhiteSpace(result.RecognizedText))
                {
                    _logger.LogError("OCR识别失败，文件id：" + fileInfo.Id);
                    return;
                }

                _logger.LogInformation("OCR识别成功，供应商：{Provider}，文件id：{FileId}", result.Provider, fileInfo.Id);
                // 查询是否存在，如果存在就替换识别的内容
                ImageText? imageText =
                    await dbContext.ImageTexts.FirstOrDefaultAsync(p => !p.IsDeleted && p.FileId == fileId);
                if (imageText == null)
                {
                    imageText = new ImageText
                    {
                        FileId = fileInfo.Id,
                        RecognizedText = result.RecognizedText,
                    };
                    SettingCommProperty.Create(imageText);
                    await dbContext.ImageTexts.AddAsync(imageText, stoppingToken);
                }
                else
                {
                    imageText.RecognizedText= result.RecognizedText;
                    SettingCommProperty.Edit(imageText);
                    dbContext.ImageTexts.Update(imageText);
                }

                await dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("OCR识别成功，文件id：" + fileInfo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OCR识别失败，文件id：" + fileId);
                throw;
            }
            finally
            {
                await Task.CompletedTask;
            }
        }, stoppingToken);
    }

}