using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 账本服务实现
/// </summary>
public class AccountBookServerImpl : IAccountBookServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _automapper;

    /// <summary>
    /// 服务提供者（用于延迟获取服务）
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 账本分享
    /// </summary>
    private readonly IAccountBookShareServer _accountBookShareServer;

    /// <summary>
    /// 账本服务构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="automapper"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="accountBookShareServer"></param>
    public AccountBookServerImpl(FinanceServiceDbContext dbContext, IMapper automapper,
        IServiceProvider serviceProvider,
        IAccountBookShareServer accountBookShareServer)
    {
        _automapper = automapper;
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _accountBookShareServer = accountBookShareServer;
    }

    /// <summary>
    /// 新增账本
    /// </summary>
    /// <param name="request">账本添加请求</param>
    /// <returns></returns>
    public long Add(AccountBookAddRequest request)
    {
        // 将请求模型映射到实体
        var accountBookEntity = _automapper.Map<AccountBook>(request);
        SettingCommProperty.Create(accountBookEntity);
        // 添加到数据库上下文
        _dbContext.AccountBooks.Add(accountBookEntity);
        // 保存更改
        _dbContext.SaveChanges();
        // 返回新增账本的ID
        return accountBookEntity.Id;
    }

    /// <summary>
    /// 删除账本
    /// </summary>
    /// <param name="id">账本id</param>
    public void Delete(long id)
    {
        // 查询要删除的账本
        var accountBook = QueryById(id);
        if (accountBook == null)
        {
            throw new NotFoundException("账本不存在，ID: " + id);
        }

        // 判断账本下是否存在记录（延迟获取服务）
        var accountingServer = _serviceProvider.GetService<IAccountingServer>();
        if (accountingServer != null && accountingServer.AccountingExistByAccountBookId(id))
        {
            throw new BusinessException("账本下存在记录，无法删除");
        }

        SettingCommProperty.Delete(accountBook);
        // 删除账本
        _dbContext.AccountBooks.Update(accountBook);
        // 保存更改
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 修改账本
    /// </summary>
    /// <param name="request"></param>
    public void Edit(AccountBookEditeRequest request)
    {
        // 查询要修改的账本
        var existingAccountBook = QueryById(request.Id);
        if (existingAccountBook == null)
        {
            throw new NotFoundException($"账本不存在，ID: {request.Id}");
        }

        // 将请求模型映射到实体
        existingAccountBook = _automapper.Map<AccountBook>(request);
        SettingCommProperty.Edit(existingAccountBook);
        // 更新账本信息
        _dbContext.AccountBooks.Update(existingAccountBook);
        // 保存更改到数据库
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 查询账本分页
    /// 分页查询当前用户创建的账本以及分享给当前用户的账本
    /// </summary>
    /// <param name="page">分页数据</param>
    /// <returns></returns>
    public PageResponse<AccountBookResponse> QueryPage(AccountBookPageRequest page)
    {
        // 获取当前会话用户（通过延迟解析）
        var contextSession = _serviceProvider.GetService<ContextSession>();
        long currentUserId = contextSession?.UserId ?? 0;

        // 构建查询：未删除，并且是当前用户创建的 OR 分享给当前用户的账本
        var query = _dbContext.AccountBooks
            .Where(ab => !ab.IsDeleted &&
                         (ab.CreateUserId == currentUserId ||
                          _dbContext.AccountBookShares.Any(sh =>
                              !sh.IsDeleted && sh.AccountBookId == ab.Id && sh.UserId == currentUserId)))
            .OrderByDescending(ab => ab.CreateDateTime)
            .AsQueryable();

        // 获取总数并分页
        var totalCount = query.Count();
        var accountBooks = query
            .Skip((page.PageIndex - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToList();

        // 获取当前页账本的统计（收入/支出）
        var accountBookIds = accountBooks.Select(ab => ab.Id).ToList();
        var stats = _dbContext.Accountings
            .Where(a => !a.IsDeleted && accountBookIds.Contains(a.AccountBookId))
            .GroupBy(a => a.AccountBookId)
            .Select(g => new
            {
                AccountBookId = g.Key,
                Income = g.Where(x => x.AfterAmount > 0).Sum(x => x.AfterAmount),
                Expenditure = g.Where(x => x.AfterAmount < 0).Sum(x => Math.Abs(x.AfterAmount))
            })
            .ToList();

        // 映射响应对象
        var accountBookResponses = _automapper.Map<List<AccountBookResponse>>(accountBooks);

        // 查询账本权限
        List<long> ids = accountBookResponses.Select(ab => ab.Id).ToList();
        Dictionary<long, PermissionTypeEnum> permissionDict = _accountBookShareServer.GetPermission(ids);

        // 将统计填充到响应对象
        foreach (var resp in accountBookResponses)
        {
            var stat = stats.FirstOrDefault(s => s.AccountBookId == resp.Id);
            resp.IncomeAmount = stat?.Income ?? 0m;
            resp.ExpenditureAmount = stat?.Expenditure ?? 0m;

            // 获取当前账本对象以判断是否是创建者
            var accountBook = accountBooks.FirstOrDefault(ab => ab.Id == resp.Id);
            long currentUserIdForPermission = contextSession?.UserId ?? 0;

            // 如果是创建者（所有者），权限为 Admin；否则从分享表查询
            if (accountBook?.CreateUserId == currentUserIdForPermission)
            {
                resp.PermissionType = PermissionTypeEnum.Admin;
            }
            else
            {
                resp.PermissionType = permissionDict.ContainsKey(resp.Id)
                    ? permissionDict[resp.Id]
                    : PermissionTypeEnum.ReadOnly; // 默认只读
            }
        }

        // 返回分页结果
        return new PageResponse<AccountBookResponse>
        {
            TotalCount = totalCount,
            PageIndex = page.PageIndex,
            PageSize = page.PageSize,
            TotalPage = (int)Math.Ceiling((double)totalCount / page.PageSize),
            Data = accountBookResponses
        };
    }

    /// <summary>
    /// 检查账本是否存在
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    public bool Exist(long accountBookId)
    {
        // 检查账本是否存在
        var accountBook = QueryById(accountBookId);
        return accountBook != null && !accountBook.IsDeleted;
    }

    /// <summary>
    /// 合并账本
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Merge(AccountBookMergeRequest request)
    {
        // 目标账本是否存在
        bool targetExist = Exist(request.TargetAccountBookId);
        if (targetExist)
        {
            throw new NotFoundException($"账本不存在，ID: {request.TargetAccountBookId}");
        }

        // 来源账本是否存在
        List<long> sourceIds = request.SourceAccountBookIds;
        List<long> notExistIds = BatchQuery(sourceIds);
        if (notExistIds.Any())
        {
            throw new NotFoundException($"以下账本不存在，ID: {string.Join(", ", notExistIds)}");
        }

        // 迁移账本下的记录
        // 规则：源账本的记录迁移到目标账本下，修改账本ID为目标账本ID
        var accountingServer = _serviceProvider.GetRequiredService<IAccountingServer>();
        accountingServer.MigrateAccountBook(request.TargetAccountBookId, sourceIds);
    }

    /// <summary>
    /// 根据id查找账本
    /// </summary>
    /// <param name="id">账本ID</param>
    /// <returns>返回账本信息</returns>
    private AccountBook? QueryById(long id)
    {
        // 查询指定ID的账本
        var accountBook = _dbContext.AccountBooks.Find(id);
        return accountBook;
    }

    /// <summary>
    /// 批量判断账本是否存在
    /// </summary>
    /// <param name="ids">账本列表</param>
    /// <returns>不存在的账本ID列表</returns>
    private List<long> BatchQuery(List<long> ids)
    {
        // 查询所有账本
        var accountBooks = _dbContext.AccountBooks.Where(p => ids.Contains(p.Id)).ToList();
        // 返回不存在的账本ID
        return ids.Where(id => !accountBooks.Any(p => p.Id == id)).ToList();
    }
}