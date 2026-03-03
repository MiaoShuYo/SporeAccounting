using Microsoft.EntityFrameworkCore;
using Quartz;
using Refit;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.RefitClient;
using SP.FinanceService.Service;

namespace SP.FinanceService.Task.SharedExpense;

/// <summary>
/// 分摊提醒定时任务
/// </summary>
public class SharedExpenseReminderWatcher : IJob
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan RepeatInterval = TimeSpan.FromDays(1);
    private readonly ISharedExpenseReminderServer _sharedExpenseReminderServer;
    private readonly FinanceServiceDbContext _dbContext;
    private readonly IInSiteNotificationsServiceApi _notificationServiceApi;
    private readonly ILogger<SharedExpenseReminderWatcher> _logger;

    public SharedExpenseReminderWatcher(
        ISharedExpenseReminderServer sharedExpenseReminderServer,
        FinanceServiceDbContext dbContext,
        IInSiteNotificationsServiceApi notificationServiceApi,
        ILogger<SharedExpenseReminderWatcher> logger)
    {
        _sharedExpenseReminderServer = sharedExpenseReminderServer;
        _dbContext = dbContext;
        _notificationServiceApi = notificationServiceApi;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.Now;
        var reminders = _sharedExpenseReminderServer.QueryPending(now);

        foreach (var reminder in reminders)
        {
            try
            {
                var sharedExpenseTitle = await _dbContext.SharedExpenses
                    .AsNoTracking()
                    .Where(x => x.Id == reminder.SharedExpenseId && !x.IsDeleted)
                    .Select(x => x.Title)
                    .FirstOrDefaultAsync();

                string title = "分摊提醒";
                string content = reminder.Content;
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = string.IsNullOrWhiteSpace(sharedExpenseTitle)
                        ? $"您有一笔分摊待处理，分摊Id：{reminder.SharedExpenseId}"
                        : $"您有一笔分摊待处理：{sharedExpenseTitle}";
                }

                var request = new SendInSiteNotificationRequest
                {
                    UserId = reminder.ParticipantId,
                    Title = title,
                    Content = content
                };

                ApiResponse<long> response = await _notificationServiceApi.SendInSiteNotification(request);
                if (response.IsSuccessStatusCode)
                {
                    RescheduleIfNeeded(reminder, now);
                }
                else
                {
                    _sharedExpenseReminderServer.Reschedule(
                        reminder.Id,
                        ReminderStatusEnum.Pending,
                        now.Add(RetryDelay),
                        now,
                        reminder.NextReminderTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送分摊提醒失败，ReminderId: {ReminderId}", reminder.Id);
                _sharedExpenseReminderServer.Reschedule(
                    reminder.Id,
                    ReminderStatusEnum.Pending,
                    now.Add(RetryDelay),
                    now,
                    reminder.NextReminderTime);
            }
        }
    }

    private void RescheduleIfNeeded(Models.Response.SharedExpenseReminderResponse reminder, DateTime now)
    {
        var nextReminderTime = reminder.NextReminderTime;
        if (!nextReminderTime.HasValue && reminder.ReminderType == ReminderTypeEnum.Overdue)
        {
            nextReminderTime = now.Add(RepeatInterval);
        }

        if (nextReminderTime.HasValue)
        {
            _sharedExpenseReminderServer.Reschedule(
                reminder.Id,
                ReminderStatusEnum.Pending,
                nextReminderTime.Value,
                now,
                nextReminderTime.Value.Add(RepeatInterval));
            return;
        }

        _sharedExpenseReminderServer.UpdateStatus(reminder.Id, ReminderStatusEnum.Sent, now);
    }
}
