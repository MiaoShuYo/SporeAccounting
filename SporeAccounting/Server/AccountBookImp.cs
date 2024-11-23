using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 账本服务实现
/// </summary>
public class AccountBookImp : IAccountBookServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly SporeAccountingDBContext _sporeAccountingDbContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sporeAccountingDbContext"></param>
    public AccountBookImp(SporeAccountingDBContext sporeAccountingDbContext)
    {
        _sporeAccountingDbContext = sporeAccountingDbContext;
    }

    /// <summary>
    /// 新增账本
    /// </summary>
    /// <param name="accountBook"></param>
    public void Add(AccountBook accountBook)
    {
        try
        {
            _sporeAccountingDbContext.AccountBooks.Add(accountBook);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 删除账本
    /// </summary>
    /// <param name="accountBookId"></param>
    public void Delete(string accountBookId)
    {
        try
        {
            AccountBook accountBook = _sporeAccountingDbContext.AccountBooks
                .FirstOrDefault(p => p.Id == accountBookId)!;
            _sporeAccountingDbContext.AccountBooks.Remove(accountBook);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 更新账本
    /// </summary>
    /// <param name="accountBook"></param>
    public void Update(AccountBook accountBook)
    {
        try
        {
            _sporeAccountingDbContext.AccountBooks.Update(accountBook);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 根据账本Id查询
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    public AccountBook Query(string accountBookId)
    {
        try
        {
            return _sporeAccountingDbContext.AccountBooks
                .FirstOrDefault(p => p.Id == accountBookId)!;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 分页查询账本
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public (int rowCount, int pageCount, IQueryable<AccountBook> accountBooks) Query(int pageNumber, int pageSize,
        string userId)
    {
        try
        {
            var query = _sporeAccountingDbContext.AccountBooks
                .Where(p => p.UserId == userId);
            int rowCount = query.Count();
            int pageCount = (int)Math.Ceiling(rowCount / (double)pageSize);
            IQueryable<AccountBook> accountBooks = query
                .Where(w => w.UserId == userId)
                .OrderByDescending(p => p.CreateDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            return (rowCount, pageCount, accountBooks);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 账本名称是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsExist(string name, string userId)
    {
        try
        {
            return _sporeAccountingDbContext.AccountBooks
                .Any(p => p.Name == name && p.UserId == userId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 账本名称是否存在，排除自己
    /// </summary>
    /// <param name="name"></param>
    /// <param name="accountBookId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsExist(string name, string accountBookId, string userId)
    {
        try
        {
            return _sporeAccountingDbContext.AccountBooks
                .Any(p => p.Name == name && p.UserId == userId && p.Id != accountBookId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 账本是否存在
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    public bool IsExistById(string accountBookId)
    {
        try
        {
            return _sporeAccountingDbContext.AccountBooks
                .Any(p => p.Id == accountBookId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 账本是否存在收支记录
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    public bool IsExistIncomeExpenditureRecord(string accountBookId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureRecords
                .Any(p => p.Id == accountBookId);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}