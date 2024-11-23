using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 收支记录实现类
/// </summary>
public class IncomeExpenditureRecordImp : IIncomeExpenditureRecordServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly SporeAccountingDBContext _sporeAccountingDbContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sporeAccountingDbContext"></param>
    public IncomeExpenditureRecordImp(SporeAccountingDBContext sporeAccountingDbContext)
    {
        _sporeAccountingDbContext = sporeAccountingDbContext;
    }

    /// <summary>
    /// 新增收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecord"></param>
    public void Add(IncomeExpenditureRecord incomeExpenditureRecord)
    {
        try
        {
            _sporeAccountingDbContext.IncomeExpenditureRecords.Add(incomeExpenditureRecord);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public void Delete(string incomeExpenditureRecordId)
    {
        throw new NotImplementedException();
    }

    public void Update(IncomeExpenditureRecord incomeExpenditureRecord)
    {
        throw new NotImplementedException();
    }

    public IncomeExpenditureRecord Query(string incomeExpenditureRecordId)
    {
        throw new NotImplementedException();
    }

    public IQueryable<IncomeExpenditureRecord> Query(int pageNumber, int pageSize, string userId, DateTime startDate,
        DateTime endDate)
    {
        throw new NotImplementedException();
    }
}