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

    /// <summary>
    /// 删除收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecordId"></param>
    /// <returns></returns>
    public void Delete(string incomeExpenditureRecordId)
    {
        try
        {
            var incomeExpenditureRecord = _sporeAccountingDbContext.IncomeExpenditureRecords
                .FirstOrDefault(x => x.Id == incomeExpenditureRecordId);
            if (incomeExpenditureRecord != null)
            {
                _sporeAccountingDbContext.IncomeExpenditureRecords.Remove(incomeExpenditureRecord);
                _sporeAccountingDbContext.SaveChanges();
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 修改收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecord"></param>
    /// <returns></returns>
    public void Update(IncomeExpenditureRecord incomeExpenditureRecord)
    {
        try
        {
            _sporeAccountingDbContext.IncomeExpenditureRecords.Update(incomeExpenditureRecord);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 查询收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecordId"></param>
    /// <returns></returns>
    public IncomeExpenditureRecord? Query(string incomeExpenditureRecordId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureRecords.FirstOrDefault(x =>
                x.Id == incomeExpenditureRecordId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 获取全部收支记录
    /// </summary>
    public List<IncomeExpenditureRecord> Query()
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureRecords.ToList();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 分页查询收支记录
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="userId"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public (int rowCount, int pageCount, List<IncomeExpenditureRecord> incomeExpenditureClassifications) Query(
        int pageNumber, int pageSize, string userId, DateTime startDate,
        DateTime endDate)
    {
        try
        {
            IQueryable<IncomeExpenditureRecord> incomeExpenditureRecords = _sporeAccountingDbContext
                .IncomeExpenditureRecords
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.RecordDate);
            int rowCount = 0;
            if (startDate != default && endDate != default)
            {
                incomeExpenditureRecords = incomeExpenditureRecords
                    .Where(x => x.RecordDate >= startDate && x.RecordDate <= endDate);
                rowCount = incomeExpenditureRecords.Count();
            }
            else
            {
                rowCount = incomeExpenditureRecords.Count();
            }

            incomeExpenditureRecords = incomeExpenditureRecords.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var pageCount = rowCount % pageSize == 0 ? rowCount / pageSize : rowCount / pageSize + 1;
            return (rowCount, pageCount, incomeExpenditureRecords.ToList());
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 收支记录是否存在
    /// </summary>
    /// <param name="incomeExpenditureRecordId"></param>
    /// <returns></returns>
    public bool IsExist(string incomeExpenditureRecordId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureRecords.Any(x => x.Id == incomeExpenditureRecordId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 更新收支记录
    /// </summary>
    /// <param name="records"></param>
    public void UpdateRecord(List<IncomeExpenditureRecord> records)
    {
        try
        {
            _sporeAccountingDbContext.IncomeExpenditureRecords.UpdateRange(records);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}