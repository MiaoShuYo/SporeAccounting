using AutoMapper;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

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

    /// <summary>
    /// 记账服务构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="autoMapper"></param>
    /// <param name="accountBookServer"></param>
    /// <param name="rabbitMqMessage"></param>
    public AccountingServerImpl(FinanceServiceDbContext dbContext, IMapper autoMapper,
        IAccountBookServer accountBookServer, RabbitMqMessage rabbitMqMessage, ICurrencyService currencyServer)
    {
        _dbContext = dbContext;
        _autoMapper = autoMapper;
        _accountBookServer = accountBookServer;
        _rabbitMqMessage = rabbitMqMessage;
        _currencyServer = currencyServer;
    }


    /// <summary>
    /// 根据账本id查询是否存在记账数据
    /// </summary>
    /// <param name="accountBookId">账本id</param>
    /// <returns></returns>
    public bool AccountingExistByAccountBookId(long accountBookId)
    {
        // 查询账本下是否有记账数据
        return _dbContext.Accountings.Any(a => a.AccountBookId == accountBookId);
    }

    /// <summary>
    /// 新增记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="request">记账添加请求</param>
    /// <returns></returns>
    public long Add(long accountBookId, AccountingAddRequest request)
    {
        AccountBookExist(accountBookId);

        // 将请求映射到实体
        var accounting = _autoMapper.Map<Accounting>(request);
        long targetCurrencyId = GetUserTargetCurrencyId();

        if (request.CurrencyId == targetCurrencyId)
        {
            accounting.AfterAmount = request.Amount;
        }
        else
        {
            accounting.AfterAmount = CalculateConvertedAmount(
                request.CurrencyId, targetCurrencyId, request.Amount);
        }

        SettingCommProperty.Create(accounting);

        // 添加到数据库
        _dbContext.Accountings.Add(accounting);
        _dbContext.SaveChanges();

        //通过MQ发送记账数据到消息队列，从预算中扣除金额
        MqPublisher mqPublisher = new MqPublisher(accounting.AfterAmount.ToString("F2"),
            MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey,
            MqQueue.BudgetQueue, MessageType.BudgetDeduct, ExchangeType.Direct);
        _rabbitMqMessage.SendAsync(mqPublisher).Start();

        // 返回新增的记账ID
        return accounting.Id;
    }

    /// <summary>
    /// 删除记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="id">记账ID</param>
    public void Delete(long accountBookId, long id)
    {
        // 检查账本是否存在
        AccountBookExist(accountBookId);
        // 查询要删除的记账记录
        Accounting accounting = QueryById(id);
        if (accounting == null)
        {
            throw new NotFoundException($"记账记录不存在，ID: {id}");
        }

        // 设置删除标志
        SettingCommProperty.Delete(accounting);
        // 保存更改
        _dbContext.Accountings.Update(accounting);
        _dbContext.SaveChanges();

        //通过MQ发送删除记账数据到消息队列，把预算中的金额恢复
        MqPublisher mqPublisher = new MqPublisher(accounting.AfterAmount.ToString("F2"),
            MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey,
            MqQueue.BudgetQueue, MessageType.BudgetAdd, ExchangeType.Direct);
        _rabbitMqMessage.SendAsync(mqPublisher).Start();
    }

    /// <summary>
    /// 修改记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="request">修改请求</param>
    public void Edit(long accountBookId, AccountingEditRequest request)
    {
        // 检查账本是否存在
        AccountBookExist(accountBookId);

        // 查询要修改的记账记录
        Accounting existingAccounting = QueryById(request.Id);
        if (existingAccounting == null)
        {
            throw new NotFoundException($"记账记录不存在，ID: {request.Id}");
        }

        // 将请求模型映射到实体
        existingAccounting = _autoMapper.Map<Accounting>(request);
        long targetCurrencyId = GetUserTargetCurrencyId();
        // 计算原金额与现金额的差额，用于更新预算
        decimal originalAmount = existingAccounting.AfterAmount;
        // 如果修改的货币与目标货币相同，直接使用原金额，否则进行转换
        if (request.CurrencyId == targetCurrencyId)
        {
            existingAccounting.AfterAmount = request.Amount;
        }
        else
        {
            existingAccounting.AfterAmount = CalculateConvertedAmount(
                request.CurrencyId, targetCurrencyId, request.Amount);
        }

        // 计算金额差额
        decimal amountDifference = originalAmount - existingAccounting.AfterAmount;

        SettingCommProperty.Edit(existingAccounting);

        // 更新记账信息
        _dbContext.Accountings.Update(existingAccounting);
        // 保存更改到数据库
        _dbContext.SaveChanges();

        // 通过MQ发送修改记账数据到消息队列，更新预算中的金额
        MqPublisher mqPublisher = new MqPublisher(amountDifference.ToString("F2"),
            MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey,
            MqQueue.BudgetQueue, MessageType.BudgetUpdate, ExchangeType.Direct);
        _rabbitMqMessage.SendAsync(mqPublisher).Start();
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
        AccountBookExist(accountBookId);

        // 查询记账记录
        Accounting accounting = QueryById(id);
        if (accounting == null)
        {
            throw new NotFoundException($"记账记录不存在，ID: {id}");
        }

        // 将实体映射到响应模型
        return _autoMapper.Map<AccountingResponse>(accounting);
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
        AccountBookExist(accountBookId);

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
    private void AccountBookExist(long accountBookId)
    {
        // 检查账本是否存在
        bool exist = _accountBookServer.Exist(accountBookId);
        if (!exist)
        {
            throw new NotFoundException("账本不存在");
        }
    }

    /// <summary>
    /// 查询记账记录
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <returns>返回记账记录</returns>
    private Accounting QueryById(long id)
    {
        // 查询记账记录
        var accounting = _dbContext.Accountings.FirstOrDefault(p => p.IsDeleted == false && p.Id == id);
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
    private decimal CalculateConvertedAmount(long sourceCurrencyId, long targetCurrencyId, decimal amount)
    {
        // 获取今日汇率记录
        var todayExchangeRate = _currencyServer
            .GetTodayExchangeRateByCode(sourceCurrencyId, targetCurrencyId);
        // 返回汇率
        decimal exchangeRate = todayExchangeRate.Result.ExchangeRate;
        // 计算转换后的金额
        return amount * exchangeRate;
    }

    /// <summary>
    /// 从用户配置中获取用户设置的目标币种
    /// </summary>
    /// <returns>返回目标币种ID</returns>
    private long GetUserTargetCurrencyId()
    {
        // TODO:从用户配置中获取用户设置的目标币种
        return 0L;
    }
}