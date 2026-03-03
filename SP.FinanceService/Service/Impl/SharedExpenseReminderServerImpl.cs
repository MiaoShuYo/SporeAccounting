using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.Common;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 分摊提醒服务实现
/// </summary>
public class SharedExpenseReminderServerImpl : ISharedExpenseReminderServer
{
    private readonly FinanceServiceDbContext _dbContext;
    private readonly IMapper _autoMapper;
    private readonly ContextSession _contextSession;

    public SharedExpenseReminderServerImpl(
        FinanceServiceDbContext dbContext,
        IMapper autoMapper,
        ContextSession contextSession)
    {
        _dbContext = dbContext;
        _autoMapper = autoMapper;
        _contextSession = contextSession;
    }

    /// <summary>
    /// 新增分摊提醒
    /// </summary>
    /// <param name="request">提醒请求</param>
    /// <returns>提醒记录Id</returns>
    public long Add(SharedExpenseReminderAddRequest request)
    {
        var entity = _autoMapper.Map<SharedExpenseReminder>(request);
        entity.ReminderId = _contextSession.UserId;
        entity.Status = ReminderStatusEnum.Pending;
        SettingCommProperty.Create(entity);
        _dbContext.SharedExpenseReminders.Add(entity);
        _dbContext.SaveChanges();
        return entity.Id;
    }

    /// <summary>
    /// 根据分摊账目Id查询提醒记录
    /// </summary>
    /// <param name="sharedExpenseId">分摊账目Id</param>
    /// <returns>提醒记录列表</returns>
    public List<SharedExpenseReminderResponse> QueryBySharedExpenseId(long sharedExpenseId)
    {
        var entities = _dbContext.SharedExpenseReminders
            .AsNoTracking()
            .Where(x => x.SharedExpenseId == sharedExpenseId && !x.IsDeleted)
            .OrderByDescending(x => x.ScheduledTime)
            .ToList();

        return _autoMapper.Map<List<SharedExpenseReminderResponse>>(entities);
    }

    /// <summary>
    /// 查询待发送提醒
    /// </summary>
    /// <param name="now">当前时间</param>
    /// <returns>提醒记录列表</returns>
    public List<SharedExpenseReminderResponse> QueryPending(DateTime now)
    {
        var entities = _dbContext.SharedExpenseReminders
            .AsNoTracking()
            .Where(x => x.Status == ReminderStatusEnum.Pending && x.ScheduledTime <= now && !x.IsDeleted)
            .OrderBy(x => x.ScheduledTime)
            .ToList();

        return _autoMapper.Map<List<SharedExpenseReminderResponse>>(entities);
    }

    /// <summary>
    /// 更新提醒状态
    /// </summary>
    /// <param name="id">提醒记录Id</param>
    /// <param name="status">提醒状态</param>
    /// <param name="sentTime">发送时间</param>
    public void UpdateStatus(long id, ReminderStatusEnum status, DateTime? sentTime = null)
    {
        var entity = _dbContext.SharedExpenseReminders
            .FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        if (entity == null)
        {
            throw new NotFoundException($"提醒记录不存在，ID: {id}");
        }

        entity.Status = status;
        entity.SentTime = sentTime;
        SettingCommProperty.Edit(entity);
        _dbContext.SharedExpenseReminders.Update(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 重新安排提醒
    /// </summary>
    /// <param name="id">提醒记录Id</param>
    /// <param name="status">提醒状态</param>
    /// <param name="scheduledTime">下次计划时间</param>
    /// <param name="sentTime">发送时间</param>
    /// <param name="nextReminderTime">下次重复提醒时间</param>
    public void Reschedule(long id, ReminderStatusEnum status, DateTime scheduledTime, DateTime? sentTime,
        DateTime? nextReminderTime)
    {
        var entity = _dbContext.SharedExpenseReminders
            .FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        if (entity == null)
        {
            throw new NotFoundException($"提醒记录不存在，ID: {id}");
        }

        entity.Status = status;
        entity.ScheduledTime = scheduledTime;
        entity.SentTime = sentTime;
        entity.NextReminderTime = nextReminderTime;
        SettingCommProperty.Edit(entity);
        _dbContext.SharedExpenseReminders.Update(entity);
        _dbContext.SaveChanges();
    }
}
