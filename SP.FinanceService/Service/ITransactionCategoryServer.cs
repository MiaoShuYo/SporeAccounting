using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Request;
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

    /// <summary>
    /// 修改收支分类
    /// </summary>
    /// <param name="category">收支分类信息</param>
    /// <returns>返回修改结果</returns>
    bool Edit(TransactionCategoryEditRequest category);

    /// <summary>
    /// 批量修改父级分类
    /// </summary>
    /// <param name="category">修改父级分类信息</param>
    /// <returns>返回修改结果</returns>
    bool EditParent(TransactionCategoryParentEditRequest category);
    /// <summary>
    /// 批量删除收支分类
    /// </summary>
    /// <param name="categoryIds">收支分类ID列表</param>
    /// <returns></returns>
    bool Delete(List<long> categoryIds);
    
    /// <summary>
    /// 查询分类信息
    /// </summary>
    /// <param name="categoryId">分类id</param>
    /// <returns>返回分类信息</returns>
    TransactionCategory?  QueryById(long categoryId);
}