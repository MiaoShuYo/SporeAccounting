using AutoMapper;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
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
    /// 记账服务接口
    /// </summary>
    private readonly IAccountingServer _accountingServer;

    /// <summary>
    /// 账本服务构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="automapper"></param>
    /// <param name="accountingServer"></param>
    public AccountBookServerImpl(FinanceServiceDbContext dbContext, IMapper automapper,
        IAccountingServer accountingServer)
    {
        _automapper = automapper;
        _dbContext = dbContext;
        _accountingServer = accountingServer;
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

        // 判断账本下是否存在记录
        if (_accountingServer.AccountingExistByAccountBookId(id))
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
    /// </summary>
    /// <param name="page">分页数据</param>
    /// <returns></returns>
    public PageResponse<AccountBookResponse> QueryPage(AccountBookPageRequest page)
    {
        // 查询符合条件的账本列表
        var query = _dbContext.AccountBooks.Where(p => !p.IsDeleted).AsQueryable();

        // 分页查询
        var totalCount = query.Count();
        var accountBooks = query.Skip((page.PageIndex - 1) * page.PageSize)
            .Take(page.PageSize).ToList();

        var accountBookResponses = _automapper.Map<List<AccountBookResponse>>(accountBooks);

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
}