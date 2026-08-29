using Microsoft.EntityFrameworkCore;
using SP.MLService.DB;
using SP.MLService.Models.Entity;
using SP.MLService.Models.Response;
using SP.MLService.Models.Request;
using AutoMapper;
using SP.Common;
using MongoDB.Driver;
using SP.Common.Model;

namespace SP.MLService.Services.Impl;

/// <summary>
/// AI使用记录服务实现
/// </summary>
public class AIUsageRecordServerImpl : IAIUsageRecordServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly MLServiceDbContext _dbContext;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="mapper">自动映射器</param>
    public AIUsageRecordServerImpl(MLServiceDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取用户AI使用记录
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>AI使用记录</returns>
    public async Task<List<AIUsageRecordResponse>> GetAIUsageRecordAsync(long userId)
    {
        var record = await _dbContext.AIUsageRecords.Where(r => r.UserId == userId).ToListAsync();
        if (record == null)
        {
            return new List<AIUsageRecordResponse>();
        }
        var response = _mapper.Map<List<AIUsageRecordResponse>>(record);
        return response;
    }

    /// <summary>
    /// 新增AI使用记录
    /// </summary>
    /// <param name="record">AI使用记录</param>
    /// <returns>新增结果</returns>
    public async Task<bool> AddAIUsageRecordAsync(AIUsageRecordRequestAdd record)
    {
        var entity = _mapper.Map<AIUsageRecord>(record);
        SettingCommProperty.Create(entity);
        await _dbContext.AIUsageRecords.AddAsync(entity);
        var result = await _dbContext.SaveChangesAsync();
        return result > 0;
    }
}