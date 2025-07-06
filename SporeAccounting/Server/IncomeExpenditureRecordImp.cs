using SporeAccounting.Models;
using SporeAccounting.Server.Interface;
using Microsoft.EntityFrameworkCore;

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
        //开启事务
        using (var transaction = _sporeAccountingDbContext.Database.BeginTransaction())
        {
            try
            {
                // 查找记录范围内的预算
                var budget = _sporeAccountingDbContext.Budgets
                    .FirstOrDefault(x => x.UserId == incomeExpenditureRecord.UserId
                                         && x.StartTime <= incomeExpenditureRecord.RecordDate &&
                                         x.EndTime >= incomeExpenditureRecord.RecordDate
                                         && x.IncomeExpenditureClassificationId ==
                                         incomeExpenditureRecord.IncomeExpenditureClassificationId);

                if (budget != null)
                {
                    // 查询分类
                    var classification = _sporeAccountingDbContext.IncomeExpenditureClassifications
                        .FirstOrDefault(x => x.Id == incomeExpenditureRecord.IncomeExpenditureClassificationId);
                    if (classification.Type
                        == IncomeExpenditureTypeEnmu.Income)
                    {
                        budget.Remaining -= incomeExpenditureRecord.AfterAmount;
                        // 获取包含支出记录记录日期的报表记录
                        var reports = _sporeAccountingDbContext.Reports
                            .Where(x => x.UserId == incomeExpenditureRecord.UserId
                                        && x.Year <= incomeExpenditureRecord.RecordDate.Year &&
                                        x.Month >= incomeExpenditureRecord.RecordDate.Month &&
                                        x.ClassificationId ==
                                        incomeExpenditureRecord.IncomeExpenditureClassificationId);
                        // 如果没有就说明程序还未将其写入报表，那么就不做任何处理
                        for (int i = 0; i < reports.Count(); i++)
                        {
                            var report = reports.ElementAt(i);
                            report.Amount += incomeExpenditureRecord.AfterAmount;
                            _sporeAccountingDbContext.Reports.Update(report);
                        }
                    }

                    _sporeAccountingDbContext.Budgets.Update(budget);
                }

                _sporeAccountingDbContext.IncomeExpenditureRecords.Add(incomeExpenditureRecord);
                _sporeAccountingDbContext.SaveChanges();
                //提交事务
                transaction.Commit();
            }
            catch (Exception e)
            {
                //回滚事务
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// 删除收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecordId"></param>
    /// <returns></returns>
    public void Delete(string incomeExpenditureRecordId)
    {
        //开启事务
        using (var transaction = _sporeAccountingDbContext.Database.BeginTransaction())
        {
            try
            {
                var incomeExpenditureRecord = _sporeAccountingDbContext.IncomeExpenditureRecords
                    .FirstOrDefault(x => x.Id == incomeExpenditureRecordId);
                if (incomeExpenditureRecord != null)
                {
                    // 查找记录范围内的预算
                    var budget = _sporeAccountingDbContext.Budgets
                        .FirstOrDefault(x => x.UserId == incomeExpenditureRecord.UserId
                                             && x.StartTime <= incomeExpenditureRecord.RecordDate &&
                                             x.EndTime >= incomeExpenditureRecord.RecordDate
                                             && x.IncomeExpenditureClassificationId == incomeExpenditureRecord
                                                 .IncomeExpenditureClassificationId);
                    if (budget != null)
                    {
                        // 查询分类
                        var classification = _sporeAccountingDbContext.IncomeExpenditureClassifications
                            .FirstOrDefault(x => x.Id == incomeExpenditureRecord.IncomeExpenditureClassificationId);
                        if (classification.Type
                            == IncomeExpenditureTypeEnmu.Income)
                        {
                            budget.Remaining += incomeExpenditureRecord.AfterAmount;
                            // 获取包含支出记录记录日期的报表记录
                            var reports = _sporeAccountingDbContext.Reports
                                .Where(x => x.UserId == incomeExpenditureRecord.UserId
                                            && x.Year <= incomeExpenditureRecord.RecordDate.Year &&
                                            x.Month >= incomeExpenditureRecord.RecordDate.Month &&
                                            x.ClassificationId ==
                                            incomeExpenditureRecord.IncomeExpenditureClassificationId);
                            // 如果没有就说明程序还未将其写入报表，那么就不做任何处理
                            for (int i = 0; i < reports.Count(); i++)
                            {
                                var report = reports.ElementAt(i);
                                report.Amount -= incomeExpenditureRecord.AfterAmount;
                                _sporeAccountingDbContext.Reports.Update(report);
                            }
                        }

                        _sporeAccountingDbContext.Budgets.Update(budget);
                    }

                    _sporeAccountingDbContext.IncomeExpenditureRecords.Remove(incomeExpenditureRecord);
                    _sporeAccountingDbContext.SaveChanges();
                    //提交事务
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                //回滚事务
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// 修改收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecord"></param>
    /// <returns></returns>
    public void Update(IncomeExpenditureRecord incomeExpenditureRecord)
    {
        using (var transaction = _sporeAccountingDbContext.Database.BeginTransaction())
        {
            try
            {
                // 查询原记录
                var oldIncomeExpenditureRecord = _sporeAccountingDbContext.IncomeExpenditureRecords
                    .FirstOrDefault(x => x.Id == incomeExpenditureRecord.Id);
                // 查找记录范围内的预算
                var budget = _sporeAccountingDbContext.Budgets
                    .FirstOrDefault(x => x.UserId == incomeExpenditureRecord.UserId
                                         && x.StartTime <= incomeExpenditureRecord.RecordDate &&
                                         x.EndTime >= incomeExpenditureRecord.RecordDate
                                         && x.IncomeExpenditureClassificationId ==
                                         incomeExpenditureRecord.IncomeExpenditureClassificationId);
                if (budget != null)
                {
                    // 查询分类
                    var classification = _sporeAccountingDbContext.IncomeExpenditureClassifications
                        .FirstOrDefault(x => x.Id == incomeExpenditureRecord.IncomeExpenditureClassificationId);
                    if (classification.Type
                        == IncomeExpenditureTypeEnmu.Income)
                    {
                        //如果是支出，需要减去原来的金额
                        budget.Remaining = (budget.Amount - incomeExpenditureRecord.AfterAmount);
                        // 根据旧的支出记录判断是否修改了记录日期
                        // 如果是修改了记录日期，那么就将原记录日期所在的报表对应的分类金额减去，将新记录日期所在报表对应的分类金额加上
                        if (oldIncomeExpenditureRecord.RecordDate != incomeExpenditureRecord.RecordDate)
                        {
                            // 获取包含支出记录记录日期的报表记录
                            var oldReports = _sporeAccountingDbContext.Reports
                                .Where(x => x.UserId == incomeExpenditureRecord.UserId
                                            && x.Year <= oldIncomeExpenditureRecord.RecordDate.Year &&
                                            x.Month >= oldIncomeExpenditureRecord.RecordDate.Month &&
                                            x.ClassificationId == oldIncomeExpenditureRecord
                                                .IncomeExpenditureClassificationId);
                            // 如果没有就说明程序还未将其写入报表，那么就不做任何处理
                            for (int i = 0; i < oldReports.Count(); i++)
                            {
                                var oldReport = oldReports.ElementAt(i);
                                oldReport.Amount -= oldIncomeExpenditureRecord.AfterAmount;
                                _sporeAccountingDbContext.Reports.Update(oldReport);
                            }

                            // 获取包含支出记录记录日期的报表记录
                            var newReport = _sporeAccountingDbContext.Reports
                                .Where(x => x.UserId == incomeExpenditureRecord.UserId
                                            && x.Year <= incomeExpenditureRecord.RecordDate.Year &&
                                            x.Month >= incomeExpenditureRecord.RecordDate.Month &&
                                            x.ClassificationId ==
                                            incomeExpenditureRecord.IncomeExpenditureClassificationId);
                            // 如果没有就说明程序还未将其写入报表，那么就不做任何处理
                            for (int i = 0; i < newReport.Count(); i++)
                            {
                                var report = newReport.ElementAt(i);
                                report.Amount += incomeExpenditureRecord.AfterAmount;
                                _sporeAccountingDbContext.Reports.Update(report);
                            }
                        }
                    }
                    else
                    {
                        //如果是收入，需要加上原来的金额
                        budget.Remaining = (budget.Amount + incomeExpenditureRecord.AfterAmount);
                    }

                    _sporeAccountingDbContext.Budgets.Update(budget);
                }

                oldIncomeExpenditureRecord.AfterAmount = incomeExpenditureRecord.AfterAmount;
                oldIncomeExpenditureRecord.BeforAmount = incomeExpenditureRecord.BeforAmount;
                oldIncomeExpenditureRecord.RecordDate = incomeExpenditureRecord.RecordDate;
                oldIncomeExpenditureRecord.Remark = incomeExpenditureRecord.Remark;
                oldIncomeExpenditureRecord.CurrencyId = incomeExpenditureRecord.CurrencyId;
                oldIncomeExpenditureRecord.IncomeExpenditureClassificationId =
                    incomeExpenditureRecord.IncomeExpenditureClassificationId;
                _sporeAccountingDbContext.IncomeExpenditureRecords.Update(oldIncomeExpenditureRecord);
                _sporeAccountingDbContext.SaveChanges();
                //提交事务
                transaction.Commit();
            }
            catch (Exception e)
            {
                //回滚事务
                transaction.Rollback();
                throw;
            }
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
    /// 获取用户的全部收支记录
    /// </summary>
    /// <param name="userId"></param>
    public List<IncomeExpenditureRecord> QueryByUserId(string userId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureRecords
                .Where(p => p.UserId == userId).ToList();
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
                .Include(x => x.IncomeExpenditureClassification) // 关联查询分类
                .Include(x => x.AccountBook) // 关联查询账簿
                .Include(x => x.Currency) // 关联查询币种
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