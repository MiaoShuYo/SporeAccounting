using AutoMapper;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 收支分类服务实现
/// </summary>
public class TransactionCategoryServer : ITransactionCategoryServer
{
    private readonly IMapper _automapper;

    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDBContext _dbContext;

    /// <summary>
    /// 收支分类服务构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    public TransactionCategoryServer(FinanceServiceDBContext dbContext, IMapper automapper)
    {
        _dbContext = dbContext;
        _automapper = automapper;
    }

    /// <summary>
    /// 查询所有收支分类
    /// </summary>
    /// <param name="parentId">父分类id</param>
    /// <returns>返回子分类列表</returns>
    public List<TransactionCategoryResponse> QueryByParentId(long parentId)
    {
        // 查询指定父分类下的所有子分类
        var categories = _dbContext.TransactionCategories
            .Where(c => c.ParentId == parentId).ToList();
        List<TransactionCategoryResponse> categoryResponses =
            _automapper.Map<List<TransactionCategoryResponse>>(categories);

        return categoryResponses;
    }
}