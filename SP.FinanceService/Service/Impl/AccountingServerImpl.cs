using System.Globalization;
using System.Threading.Tasks;
using System.Text.Json;
using AutoMapper;
using Refit;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Model;
using SP.Common.Model.Enumeration;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Mq.Models;
using SP.FinanceService.RefitClient;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 记账服务实现类
/// </summary>
public class AccountingServerImpl : IAccountingServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;

    /// <summary>
    /// 账本服务
    /// </summary>
    private readonly IAccountBookServer _accountBookServer;

    /// <summary>
    /// 货币服务
    /// </summary>
    private readonly ICurrencyService _currencyServer;

    /// <summary>
    /// RabbitMQ消息处理
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _autoMapper;

    ///<summary>
    /// 用户配置接口
    ///</summary>
    private readonly IConfigServiceApi _configService;

    /// <summary>
    /// 上下文会话
    /// </summary>
    private readonly ContextSession _contextSession;

    /// <summary>
    /// 记账服务构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="autoMapper"></param>
    /// <param name="configService"></param>
    /// <param name="accountBookServer"></param>
    /// <param name="rabbitMqMessage"></param>
    /// <param name="currencyServer"></param>
    /// <param name="contextSession"></param>
    public AccountingServerImpl(FinanceServiceDbContext dbContext, IMapper autoMapper,
        IConfigServiceApi configService, IAccountBookServer accountBookServer, RabbitMqMessage rabbitMqMessage,
        ICurrencyService currencyServer, ContextSession contextSession)
    {
        _dbContext = dbContext;
        _autoMapper = autoMapper;
        _accountBookServer = accountBookServer;
        _rabbitMqMessage = rabbitMqMessage;
        _currencyServer = currencyServer;
        _configService = configService;
        _contextSession = contextSession;
    }


    /// <summary>
    /// 根据账本id查询是否存在记账数据
    /// </summary>
    /// <param name="accountBookId">账本id</param>
    /// <returns></returns>
    public bool AccountingExistByAccountBookId(long accountBookId)
    {
        // 查询账本下是否有记账数据
        return _dbContext.Accountings.Any(a => a.AccountBookId == accountBookId && !a.IsDeleted);
    }

    /// <summary>
    /// 新增记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="request">记账添加请求</param>
    /// <returns></returns>
    public async System.Threading.Tasks.Task<long> Add(long accountBookId, AccountingAddRequest request)
    {
        AccountBookExist(accountBookId, true);
        ValidatePaymentMethod(request.PaymentMethodId!.Value);

        // 将请求映射到实体
        var accounting = _autoMapper.Map<Accounting>(request);
        long targetCurrencyId = await GetUserTargetCurrencyId();

        if (request.CurrencyId == targetCurrencyId)
        {
            accounting.AfterAmount = request.Amount!.Value;
        }
        else
        {
            accounting.AfterAmount = await CalculateConvertedAmount(
                request.CurrencyId!.Value, targetCurrencyId, request.Amount!.Value);
        }

        SettingCommProperty.Create(accounting);

        // 添加到数据库
        _dbContext.Accountings.Add(accounting);
        _dbContext.SaveChanges();

        // 组合预算变动消息：消费金额、消费类型
        BudgetChangeMQ budgetChange = new BudgetChangeMQ
        {
            ChangeAmount = accounting.AfterAmount,
            TransactionCategoryId = request.TransactionCategoryId!.Value,
            UserId = _contextSession.UserId
        };
        string budgetChangeJson = JsonSerializer.Serialize(budgetChange);


        //通过MQ发送记账数据到消息队列，从预算中扣除金额
        MqPublisher mqPublisher = new MqPublisher(budgetChangeJson,
            MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey,
            MqQueue.BudgetQueue, MessageType.BudgetDeduct, ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(mqPublisher);

        // 返回新增的记账ID
        return accounting.Id;
    }

    /// <summary>
    /// 删除记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="id">记账ID</param>
    public async System.Threading.Tasks.Task Delete(long accountBookId, long id)
    {
        // 检查账本是否存在
        AccountBookExist(accountBookId, true);
        // 查询要删除的记账记录
        Accounting accounting = QueryAccountingById(id, accountBookId);
        if (accounting == null)
        {
            throw new NotFoundException($"记账记录不存在，ID: {id}");
        }

        // 设置删除标志
        SettingCommProperty.Delete(accounting);
        // 保存更改
        _dbContext.Accountings.Update(accounting);
        _dbContext.SaveChanges();

        // 组合预算变动消息：消费金额、消费类型
        BudgetChangeMQ budgetChange = new BudgetChangeMQ
        {
            ChangeAmount = accounting.AfterAmount,
            TransactionCategoryId = accounting.TransactionCategoryId,
            UserId = _contextSession.UserId
        };
        string budgetChangeJson = JsonSerializer.Serialize(budgetChange);

        //通过MQ发送删除记账数据到消息队列，把预算中的金额恢复
        MqPublisher mqPublisher = new MqPublisher(budgetChangeJson,
            MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey,
            MqQueue.BudgetQueue, MessageType.BudgetAdd, ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(mqPublisher);
    }

    /// <summary>
    /// 修改记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="request">修改请求</param>
    public async System.Threading.Tasks.Task Edit(long accountBookId, AccountingEditRequest request)
    {
        // 检查账本是否存在
        AccountBookExist(accountBookId, true);
        ValidatePaymentMethod(request.PaymentMethodId!.Value);

        // 查询要修改的记账记录
        Accounting existingAccounting = QueryAccountingById(request.Id!.Value, accountBookId);
        if (existingAccounting == null)
        {
            throw new NotFoundException($"记账记录不存在，ID: {request.Id}");
        }

        // 保存原始金额，用于后续计算差额
        decimal originalAmount = existingAccounting.AfterAmount;
        // 将请求模型映射到已追踪实体（不替换整个对象）
        _autoMapper.Map(request, existingAccounting);
        long targetCurrencyId = await GetUserTargetCurrencyId();
        // 如果修改的货币与目标货币相同，直接使用原金额，否则进行转换
        if (request.CurrencyId == targetCurrencyId)
        {
            existingAccounting.AfterAmount = request.Amount!.Value;
        }
        else
        {
            existingAccounting.AfterAmount = await CalculateConvertedAmount(
                request.CurrencyId!.Value, targetCurrencyId, request.Amount!.Value);
        }

        // 计算金额差额
        decimal amountDifference = originalAmount - existingAccounting.AfterAmount;

        SettingCommProperty.Edit(existingAccounting);

        // 更新记账信息
        _dbContext.Accountings.Update(existingAccounting);
        // 保存更改到数据库
        _dbContext.SaveChanges();

        // 通过MQ发送修改记账数据到消息队列，更新预算中的金额
        BudgetChangeMQ budgetUpdateChange = new BudgetChangeMQ
        {
            ChangeAmount = amountDifference,
            TransactionCategoryId = request.TransactionCategoryId!.Value,
            UserId = _contextSession.UserId
        };
        MqPublisher mqPublisher = new MqPublisher(JsonSerializer.Serialize(budgetUpdateChange),
            MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey,
            MqQueue.BudgetQueue, MessageType.BudgetUpdate, ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(mqPublisher);
    }

    /// <summary>
    /// 查询记账详情
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="id">记账记录id</param>
    /// <returns>记账详情</returns>
    public AccountingResponse QueryById(long accountBookId, long id)
    {
        // 检查账本是否存在
        AccountBookExist(accountBookId, false);

        // 查询记账记录
        Accounting accounting = QueryAccountingById(id, accountBookId);
        if (accounting == null)
        {
            throw new NotFoundException($"记账记录不存在，ID: {id}");
        }

        // 将实体映射到响应模型
        var response = _autoMapper.Map<AccountingResponse>(accounting);
        response.PaymentMethodName = _dbContext.PaymentMethods
            .Where(p => p.Id == accounting.PaymentMethodId && !p.IsDeleted)
            .Select(p => p.Name)
            .FirstOrDefault() ?? string.Empty;
        return response;
    }

    /// <summary>
    /// 查询记账分页
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="page">分页请求</param>
    /// <returns>记账列表</returns>
    public PageResponse<AccountingResponse> QueryPage(long accountBookId, AccountingPageRequest page)
    {
        // 检查账本是否存在
        AccountBookExist(accountBookId, false);

        // 查询记账记录分页
        var query = _dbContext.Accountings
            .Where(a => a.IsDeleted == false && a.AccountBookId == accountBookId)
            .OrderByDescending(a => a.RecordDate);

        // 分页查询
        var pagedData = query.Skip((page.PageIndex - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToList();

        // 获取总记录数
        int totalCount = query.Count();

        // 将实体列表映射到响应模型列表
        var responseList = _autoMapper.Map<List<AccountingResponse>>(pagedData);

        // 填充支付方式名称
        var paymentMethodIds = pagedData.Select(a => a.PaymentMethodId).Distinct().ToList();
        var paymentMethodNames = _dbContext.PaymentMethods
            .Where(p => paymentMethodIds.Contains(p.Id) && !p.IsDeleted)
            .ToDictionary(p => p.Id, p => p.Name);
        foreach (var item in responseList)
        {
            item.PaymentMethodName = paymentMethodNames.GetValueOrDefault(item.PaymentMethodId) ?? string.Empty;
        }

        // 返回分页响应
        return new PageResponse<AccountingResponse>
        {
            TotalCount = totalCount,
            TotalPage = (int)Math.Ceiling((double)totalCount / page.PageSize),
            PageIndex = page.PageIndex,
            PageSize = page.PageSize,
            Data = responseList
        };
    }

    /// <summary>
    /// 检查账本是否存在
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    private void AccountBookExist(long accountBookId, bool requireWritePermission)
    {
        var accountBook = _dbContext.AccountBooks
            .FirstOrDefault(p => p.Id == accountBookId && !p.IsDeleted);
        if (accountBook == null)
        {
            throw new NotFoundException("账本不存在");
        }

        bool isOwner = accountBook.CreateUserId == _contextSession.UserId;
        if (isOwner)
        {
            return;
        }

        var sharePermission = _dbContext.AccountBookShares
            .Where(p => !p.IsDeleted
                        && p.AccountBookId == accountBookId
                        && p.UserId == _contextSession.UserId)
            .Select(p => p.PermissionType)
            .FirstOrDefault();

        bool hasReadPermission = sharePermission == PermissionTypeEnum.ReadOnly
                                 || sharePermission == PermissionTypeEnum.ReadWrite
                                 || sharePermission == PermissionTypeEnum.Admin;
        bool hasWritePermission = sharePermission == PermissionTypeEnum.ReadWrite
                                  || sharePermission == PermissionTypeEnum.Admin;

        if (!hasReadPermission || (requireWritePermission && !hasWritePermission))
        {
            throw new ForbiddenException("无权访问账本");
        }
    }

    /// <summary>
    /// 查询记账记录
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <returns>返回记账记录</returns>
    private Accounting QueryAccountingById(long id, long accountBookId)
    {
        // 查询记账记录
        var accounting = _dbContext.Accountings.FirstOrDefault(p =>
            p.IsDeleted == false && p.Id == id && p.AccountBookId == accountBookId);
        if (accounting == null)
        {
            throw new NotFoundException($"记账记录不存在，ID: {id}");
        }

        return accounting;
    }

    /// <summary>
    /// 计算源货币和目标货币转换后的金额
    /// </summary>
    /// <param name="sourceCurrencyId">源货币ID</param>
    /// <param name="targetCurrencyId">目标货币ID</param>
    /// <returns>返回转换后的金额</returns>
    private async System.Threading.Tasks.Task<decimal> CalculateConvertedAmount(long sourceCurrencyId, long targetCurrencyId, decimal amount)
    {
        // 获取今日汇率记录
        var todayExchangeRate = await _currencyServer
            .GetTodayExchangeRateByCode(sourceCurrencyId, targetCurrencyId);
        // 返回汇率
        decimal exchangeRate = todayExchangeRate.ExchangeRate;
        // 计算转换后的金额
        return amount * exchangeRate;
    }

    /// <summary>
    /// 从用户配置中获取用户设置的目标币种
    /// </summary>
    /// <returns>返回目标币种ID</returns>
    private async System.Threading.Tasks.Task<long> GetUserTargetCurrencyId()
    {
        ApiResponse<ConfigResponse> apiResponse = await _configService.QueryByType(ConfigTypeEnum.Currency);
        // 检查响应是否成功，并且内容不为空
        if (apiResponse.IsSuccessStatusCode && apiResponse.Content != null)
        {
            if (long.TryParse(apiResponse.Content.Value, out var currencyId))
            {
                return currencyId;
            }

            throw new BusinessException("用户默认币种配置无效");
        }

        throw new RefitException($"获取用户默认币种失败: {apiResponse.StatusCode}");
    }

    /// <summary>
    /// 根据时间范围获取记账记录列表
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public object GetAccountingsByTimeRange(DateTime startTime, DateTime endTime)
    {
        // 查询记账记录
        var accountings = _dbContext.Accountings
            .Where(a => a.IsDeleted == false
                        && a.RecordDate >= startTime
                        && a.RecordDate <= endTime
                        && a.CreateUserId == _contextSession.UserId)
            .ToList();
        // 将实体列表映射到响应模型列表
        var responseList = _autoMapper.Map<List<AccountingResponse>>(accountings);
        // 返回响应列表
        return responseList;
    }

    /// <summary>
    /// 合并记账记录到目标账本
    /// </summary>
    /// <param name="targetAccountBookId"></param>
    /// <param name="sourceIds"></param>
    public void MigrateAccountBook(long targetAccountBookId, List<long> sourceIds)
    {
        // 迁移记账记录到目标账本
        var accountingsToMigrate = _dbContext.Accountings
            .Where(a => sourceIds.Contains(a.AccountBookId) && a.IsDeleted == false)
            .ToList();
        foreach (var accounting in accountingsToMigrate)
        {
            accounting.AccountBookId = targetAccountBookId;
            SettingCommProperty.Edit(accounting);
        }

        _dbContext.Accountings.UpdateRange(accountingsToMigrate);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 校验支付方式是否存在且属于当前用户
    /// </summary>
    /// <param name="paymentMethodId">支付方式ID</param>
    private void ValidatePaymentMethod(long paymentMethodId)
    {
        long userId = _contextSession.UserId;
        bool exists = _dbContext.PaymentMethods
            .Any(p => p.Id == paymentMethodId && p.CreateUserId == userId && !p.IsDeleted);
        if (!exists)
        {
            throw new NotFoundException("支付方式不存在", paymentMethodId);
        }
    }
}