using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 账本分享服务实现类
/// </summary>
public class AccountBookShareServerImpl : IAccountBookShareServer
{
    /// <summary>
    /// 账本服务
    /// </summary>
    private readonly IAccountBookServer _accountBookServer;

    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _automapper;

    /// <summary>
    /// 上下文会话
    /// </summary>
    private readonly ContextSession _contextSession;

    /// <summary>
    /// RabbitMQ消息服务
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 账本分享服务构造函数
    /// </summary>
    /// <param name="accountBookServer"></param>
    /// <param name="dbContext"></param>
    /// <param name="automapper"></param>
    /// <param name="contextSession"></param>
    /// <param name="rabbitMqMessage"></param>
    public AccountBookShareServerImpl(IAccountBookServer accountBookServer,
        FinanceServiceDbContext dbContext,
        IMapper automapper, ContextSession contextSession,
        RabbitMqMessage rabbitMqMessage)
    {
        _accountBookServer = accountBookServer;
        _dbContext = dbContext;
        _automapper = automapper;
        _contextSession = contextSession;
        _rabbitMqMessage = rabbitMqMessage;
    }

    /// <summary>
    /// 共享账本
    /// </summary>
    /// <param name="request"></param>
    public async System.Threading.Tasks.Task Share(AccountBookShareAddRequest request)
    {
        // 账本是否存在
        bool accountBookExist = _accountBookServer.Exist(request.AccountBookId);
        if (!accountBookExist)
        {
            throw new NotFoundException("账本不存在");
        }

        // 校验当前用户是否为账本创建者（只有账本创建者才能分享）
        var accountBookOwner = _dbContext.AccountBooks
            .FirstOrDefault(ab => ab.Id == request.AccountBookId && !ab.IsDeleted && ab.CreateUserId == _contextSession.UserId);
        if (accountBookOwner == null)
        {
            throw new ForbiddenException("无权分享此账本，仅账本创建者可进行分享");
        }

        // 存储共享账本
        var accountBookShares = _automapper.Map<List<AccountBookShare>>(request);
        foreach (var accountBookShare in accountBookShares)
        {
            SettingCommProperty.Create(accountBookShare);
        }

        _dbContext.AccountBookShares.AddRange(accountBookShares);
        await _dbContext.SaveChangesAsync();
        var accountBook = _accountBookServer.Get(request.AccountBookId);
        // 发送账本分享通知
        string message = $"用户{_contextSession.UserName}向你分享了{accountBook.Name}账本，赶紧去看看吧！";
        //通过MQ发送记账数据到消息队列，从预算中扣除金额
        MqPublisher mqPublisher = new MqPublisher(message,
            MqExchange.AccountBookShareExchange,
            MqRoutingKey.AccountBookShareRoutingKey,
            MqQueue.AccountBookShareQueue, MessageType.AccountBookShare, ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(mqPublisher);
    }

    /// <summary>
    /// 分页获取共享账本列表
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public PageResponse<AccountBookShareResponse> Page(AccountBookSharePageRequest request)
    {
        var query = _dbContext.AccountBookShares.Where(p => !p.IsDeleted && p.CreateUserId == _contextSession.UserId)
            .OrderByDescending(o => o.CreateDateTime)
            .AsNoTracking();
        // 获取总数
        int totalCount = query.Count();
        // 分页查询
        var data = query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
        // 映射响应对象
        var responseData = _automapper.Map<List<AccountBookShareResponse>>(data);
        // 构建分页响应
        var pageResponse = new PageResponse<AccountBookShareResponse>
        {
            TotalCount = totalCount,
            Data = responseData,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
        return pageResponse;
    }

    /// <summary>
    /// 分页获取共享给我的账本列表
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public PageResponse<AccountBookShareResponse> PageSharesToMe(AccountBookSharePageRequest request)
    {
        var query = _dbContext.AccountBookShares.Where(p => !p.IsDeleted && p.UserId == _contextSession.UserId)
            .OrderByDescending(o => o.CreateDateTime)
            .AsNoTracking();
        // 获取总数
        int totalCount = query.Count();
        // 分页查询
        var data = query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // 映射响应对象
        var responseData = _automapper.Map<List<AccountBookShareResponse>>(data);
        // 构建分页响应
        var pageResponse = new PageResponse<AccountBookShareResponse>
        {
            TotalCount = totalCount,
            Data = responseData,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
        return pageResponse;
    }

    /// <summary>
    /// 撤销共享
    /// </summary>
    /// <param name="request"></param>
    public async System.Threading.Tasks.Task Revoke(AccountBookRevokeSharingRequest request)
    {
        // 校验当前用户是否为账本创建者
        var accountBookOwner = _dbContext.AccountBooks
            .FirstOrDefault(ab => ab.Id == request.AccountBookId && !ab.IsDeleted && ab.CreateUserId == _contextSession.UserId);
        if (accountBookOwner == null)
        {
            throw new ForbiddenException("无权撤销此账本的分享，仅账本创建者可撤销");
        }

        var shares = _dbContext.AccountBookShares
            .Where(p => p.AccountBookId == request.AccountBookId && request.UserIds.Contains(p.UserId) && !p.IsDeleted)
            .ToList();
        if (!shares.Any())
        {
            throw new NotFoundException("未找到共享记录");
        }

        foreach (var share in shares)
        {
            SettingCommProperty.Delete(share);
        }

        await _dbContext.SaveChangesAsync();

        var accountBook = _accountBookServer.Get(request.AccountBookId);
        // 发送账本分享通知
        string message = $"用户{_contextSession.UserName}撤销了对{accountBook.Name}账本的分享！";
        //通过MQ发送记账数据到消息队列，从预算中扣除金额
        MqPublisher mqPublisher = new MqPublisher(message,
            MqExchange.AccountBookShareExchange,
            MqRoutingKey.AccountBookShareRoutingKey,
            MqQueue.AccountBookShareQueue, MessageType.AccountBookShareRevoke, ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(mqPublisher);
    }

    /// <summary>
    /// 编辑共享
    /// </summary>
    /// <param name="request"></param>
    public async System.Threading.Tasks.Task Edit(AccountBookShareEditRequest request) 
    {
        // 校验当前用户是否为账本创建者
        var accountBookOwner = _dbContext.AccountBooks
            .FirstOrDefault(ab => ab.Id == request.AccountBookId && !ab.IsDeleted && ab.CreateUserId == _contextSession.UserId);
        if (accountBookOwner == null)
        {
            throw new ForbiddenException("无权修改此账本的分享权限，仅账本创建者可修改");
        }

        var shares = _dbContext.AccountBookShares
            .Where(p => p.AccountBookId == request.AccountBookId && request.UserIds.Contains(p.UserId) && !p.IsDeleted)
            .ToList();
        if (!shares.Any())
        {
            throw new NotFoundException("未找到共享记录");
        }

        foreach (var share in shares)
        {
            share.PermissionType = request.PermissionType;
            SettingCommProperty.Edit(share);
        }

        _dbContext.SaveChanges();

        var accountBook = _accountBookServer.Get(request.AccountBookId);
        // 发送账本分享通知
        string message = $"用户{_contextSession.UserName}修改了你对{accountBook.Name}账本的权限！";
        //通过MQ发送记账数据到消息队列，从预算中扣除金额
        MqPublisher mqPublisher = new MqPublisher(message,
            MqExchange.AccountBookShareExchange,
            MqRoutingKey.AccountBookShareRoutingKey,
            MqQueue.AccountBookShareQueue, MessageType.AccountBookSharePermissionUpdate, ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(mqPublisher);
    }

    /// <summary>
    /// 根据账本id集合查询账本权限
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public Dictionary<long, PermissionTypeEnum> GetPermission(List<long> ids)
    {
        return _dbContext.AccountBookShares
            .Where(p => ids.Contains(p.AccountBookId) && !p.IsDeleted)
            .ToDictionary(p => p.AccountBookId, p => p.PermissionType);
    }
}