using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 收支分类数据库操作接口
/// </summary>
public interface IIncomeExpenditureClassificationServer
{
    /// <summary>
    /// 新增收支分类
    /// </summary>
    /// <param name="classification"></param>
    void Add(IncomeExpenditureClassification classification);

    /// <summary>
    /// 删除收支分类（物理删除）
    /// </summary>
    /// <param name="classificationId"></param>
    void Delete(string classificationId);

    /// <summary>
    /// 修改收支分类
    /// </summary>
    /// <param name="classification"></param>
    void Update(IncomeExpenditureClassification classification);

    /// <summary>
    /// 根据父级分类Id查询
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    IQueryable<IncomeExpenditureClassification> Query(string parentId);

    /// <summary>
    /// 分页查询收支分类
    /// </summary>
    /// <param name="sysRoleUrlPageViewModel"></param>
    /// <returns></returns>
    (int rowCount, int pageCount, List<IncomeExpenditureClassification> incomeExpenditureClassifications)
        GetByPage(IncomeExpenditureClassificationPageViewModel sysRoleUrlPageViewModel);

    /// <summary>
    /// 分类名称是否存在
    /// </summary>
    /// <param name="classificationName"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool IsExist(string classificationName, string userId);
}