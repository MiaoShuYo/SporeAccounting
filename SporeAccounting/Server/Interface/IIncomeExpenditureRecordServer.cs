using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 收支记录服务接口
/// </summary>
public interface IIncomeExpenditureRecordServer
{
    /// <summary>
    /// 新增收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecord"></param>
    void Add(IncomeExpenditureRecord incomeExpenditureRecord);

    /// <summary>
    /// 删除收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecordId"></param>
    void Delete(string incomeExpenditureRecordId);

    /// <summary>
    /// 修改收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecord"></param>
    void Update(IncomeExpenditureRecord incomeExpenditureRecord);

    /// <summary>
    /// 查询收支记录
    /// </summary>
    /// <param name="incomeExpenditureRecordId"></param>
    /// <returns></returns>
    IncomeExpenditureRecord? Query(string incomeExpenditureRecordId);

    /// <summary>
    /// 分页查询收支记录
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="userId"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    (int rowCount, int pageCount, List<IncomeExpenditureRecord> incomeExpenditureClassifications) Query(int pageNumber,
        int pageSize, string userId, DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// 收支记录是否存在
    /// </summary>
    /// <param name="incomeExpenditureRecordId"></param>
    /// <returns></returns>
    bool IsExist(string incomeExpenditureRecordId);
}