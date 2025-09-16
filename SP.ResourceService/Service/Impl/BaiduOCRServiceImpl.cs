using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.ResourceService.DB;
using SP.ResourceService.Models.Entity;

namespace SP.ResourceService.Service.Impl;

/// <summary>
/// 百度OCR服务实现
/// </summary>
public class BaiduOCRServiceImpl : IOCRService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<BaiduOCRServiceImpl> _logger;

    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly ResourceServiceDbContext _dbContext;

    /// <summary>
    /// RabbitMQ消息服务
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="rabbitMqMessage"></param>
    public BaiduOCRServiceImpl(ILogger<BaiduOCRServiceImpl> logger,
        ResourceServiceDbContext dbContext, RabbitMqMessage rabbitMqMessage)
    {
        _logger = logger;
        _dbContext = dbContext;
        _rabbitMqMessage = rabbitMqMessage;
    }

    /// <summary>
    /// 识别图片中的文字
    /// </summary>
    /// <param name="fileId">图片文件id</param>
    /// <returns></returns>
    public async Task RecognizeTextAsync(long fileId)
    {
        // 校验图片是否存在
        Files? file = await _dbContext.Files.FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == fileId);
        if (file == null)
        {
            throw new NotFoundException("文件不存在");
        }

        // 格式必须是PNG、JPG或JPEG
        if (file.ContentType != "image/png" && file.ContentType != "image/jpg" && file.ContentType != "image/jpeg")
        {
            throw new BadRequestException("仅支持PNG、JPG或JPEG格式的图片");
        }

        string fileInfoJson = JsonSerializer.Serialize(file);
        MqPublisher publisher = new MqPublisher(fileInfoJson, MqExchange.MessageExchange,
            MqRoutingKey.OCRRoutingKey, MqQueue.OCRQueue, "", ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(publisher);
    }

    /// <summary>
    /// 获取识别到的图片文字
    /// </summary>
    /// <param name="fileId">图片文件id</param>
    /// <returns></returns>
    public async Task<string?> GetRecognizedTextAsync(long fileId)
    {
        string? text =await _dbContext.ImageTexts.Where(p => !p.IsDeleted && p.FileId == fileId)
            .Select(p => p.RecognizedText).FirstOrDefaultAsync();
        if (text == null)
        {
            return "";
        }
        return text;
    }
}