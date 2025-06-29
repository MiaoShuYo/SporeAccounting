using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 收支分类服务接口
/// </summary>
public interface ITransactionCategoryServer
{
    /// <summary>
    /// 查询所有收支分类
    /// </summary>
    /// <param name="parentId">父分类id</param>
    /// <returns>返回子分类列表</returns>
    List<TransactionCategoryResponse> QueryByParentId(long parentId);
}