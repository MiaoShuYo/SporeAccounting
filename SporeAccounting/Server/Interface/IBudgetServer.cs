using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 预算服务
/// </summary>
public interface IBudgetServer
{
    /// <summary>
    /// 添加预算
    /// </summary>
    /// <param name="budget"></param>
    void Add(Budget budget);

    /// <summary>
    /// 删除预算
    /// </summary>
    /// <param name="id"></param>
    void Delete(string id);

    /// <summary>
    /// 修改预算
    /// </summary>
    /// <param name="budget"></param>
    void Update(Budget budget);

    /// <summary>
    /// 修改预算
    /// </summary>
    /// <param name="budgets"></param>
    void Update(List<Budget> budgets);

    /// <summary>
    /// 查询预算
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    List<Budget> Query(string userId);

    /// <summary>
    /// 用户是否存在该类型预算
    /// </summary>
    /// <param name="classificationId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool IsExistByClassificationId(string classificationId, string userId);
    
    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool IsExist(string id);
    /// <summary>
    /// 是否是当前用户的
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool IsYou(string id, string userId);
}