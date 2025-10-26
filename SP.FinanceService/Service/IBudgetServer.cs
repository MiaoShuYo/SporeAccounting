using SP.Common.Model;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 预算服务器接口
/// </summary>
public interface IBudgetServer
{
    /// <summary>
    /// 新增预算
    /// </summary>
    /// <param name="budget">预算</param>
    /// <returns>预算id</returns>
    long Add(BudgetAddRequest budget);

    /// <summary>
    /// 删除预算
    /// </summary>
    /// <param name="id">预算id</param>
    void Delete(long id);

    /// <summary>
    /// 修改预算
    /// </summary>
    /// <param name="budget">修改预算</param>
    void Edit(BudgetEditRequest budget);

    /// <summary>
    /// 查询预算分页列表
    /// </summary>
    /// <param name="request">分页查询</param>
    /// <returns>预算列表</returns>
    PageResponse<BudgetResponse> QueryPage(BudgetPageRequest request);

    /// <summary>
    /// 查询预算信息
    /// </summary>
    /// <param name="id">预算id</param>
    /// <returns>预算信息</returns>
    BudgetResponse QueryById(long id);

    /// <summary>
    /// 查询当前在用的预算列表
    /// </summary>
    /// <returns>预算列表</returns>
    List<Budget> QueryCurrentBudgets();

    /// <summary>
    /// 根据支出分类获取当前用户在用的预算列表
    /// </summary>
    /// <param name="transactionCategoryId">收支分类id</param>
    /// <param name="userId">用户id</param>
    /// <returns>预算列表</returns>
    List<Budget> QueryCurrentBudgetsByExpenseCategoryId(long transactionCategoryId, long userId);

    /// <summary>
    /// 更新预算列表
    /// </summary>
    /// <param name="budgets"></param>
    void UpdateBudgets(List<Budget> budgets);

    /// <summary>
    /// 查询指定日期的全部预算（用于Task）
    /// </summary>
    /// <param name="dateTime">指定日期</param>
    /// <returns>预算列表</returns>
    IQueryable<Budget> QueryBudgetsByDate(DateTime dateTime);

    /// <summary>
    /// 查询当前用户正在使用的预算列表
    /// </summary>
    /// <returns></returns>
    List<BudgetResponse> QueryActiveBudgets();
}