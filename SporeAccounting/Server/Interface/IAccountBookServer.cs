using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 账本服务接口
/// </summary>
public interface IAccountBookServer
{
    /// <summary>
    /// 新增账本
    /// </summary>
    /// <param name="accountBook"></param>
    void Add(AccountBook accountBook);

    /// <summary>
    /// 删除账本
    /// </summary>
    /// <param name="accountBookId"></param>
    void Delete(string accountBookId);

    /// <summary>
    /// 修改账本
    /// </summary>
    /// <param name="accountBook"></param>
    void Update(AccountBook accountBook);

    /// <summary>
    /// 查询账本
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    AccountBook Query(string accountBookId);

    /// <summary>
    /// 分页查询账本
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    (int rowCount, int pageCount, IQueryable<AccountBook> accountBooks)  Query(int pageNumber, int pageSize, string userId);
    
    /// <summary>
    /// 账本名称是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool IsExist(string name, string userId);
    /// <summary>
    /// 账本名称是否存在，排除自己
    /// </summary>
    /// <param name="name"></param>
    /// <param name="accountBookId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool IsExist(string name,string accountBookId, string userId);

    /// <summary>
    /// 账本是否存在
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    bool IsExistById(string accountBookId);

    /// <summary>
    /// 账本是否存在收支记录
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    bool IsExistIncomeExpenditureRecord(string accountBookId);
}