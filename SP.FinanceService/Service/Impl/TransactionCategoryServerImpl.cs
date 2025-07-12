using AutoMapper;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 收支分类服务实现
/// </summary>
public class TransactionCategoryServerImpl : ITransactionCategoryServer
{
    private readonly IMapper _automapper;

    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;

    /// <summary>
    /// 收支分类服务构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    public TransactionCategoryServerImpl(FinanceServiceDbContext dbContext, IMapper automapper)
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

    /// <summary>
    /// 修改收支分类
    /// </summary>
    /// <param name="category">收支分类信息</param>
    /// <returns>返回修改结果</returns>
    public bool Edit(TransactionCategoryEditRequest category)
    {
        // 查询要修改的分类
        var existingCategory = QueryById(category.Id);

        if (existingCategory == null)
        {
            throw new NotFoundException($"分类不存在，ID: {category.Id}");
        }

        existingCategory.Name = category.Name;
        // 保存更改到数据库
        _dbContext.SaveChanges();
        return true;
    }

    /// <summary>
    /// 批量修改父级分类
    /// </summary>
    /// <param name="category">修改父级分类信息</param>
    /// <returns>返回修改结果</returns>
    public bool EditParent(TransactionCategoryParentEditRequest category)
    {
        List<TransactionCategory> existingCategories = QueryByIds(category.Id);
        if (existingCategories == null || !existingCategories.Any())
        {
            throw new NotFoundException("未找到指定的收支分类");
        }

        // 父级分类是否存在
        var parentCategory = QueryById(category.ParentId);
        if (parentCategory == null)
        {
            throw new NotFoundException($"父级分类不存在，ID: {category.ParentId}");
        }

        // 判断是否存在不能修改的分类
        var cannotEdit = existingCategories.Where(c => !c.CanDelete).ToList();
        if (cannotEdit.Any())
        {
            var names = string.Join("，", cannotEdit.Select(c => c.Name));
            // 抛出业务异常，提示不能修改的分类名称
            throw new BusinessException($"以下分类不能修改父级分类：{names}");
        }

        // 检查是否将分类的父级ID设置为自身ID，防止循环引用
        if (existingCategories.Any(c => c.Id == category.ParentId))
        {
            throw new BusinessException("不能将分类的父级ID设置为自身ID，防止循环引用");
        }

        // 检查父级分类与子分类的类型是否一致
        if (existingCategories.Any(c => c.Type != parentCategory.Type))
        {
            throw new BusinessException("修改父类时不能指定不同类型的分类作为父类");
        }

        // 开启事务
        using var transaction = _dbContext.Database.BeginTransaction();

        // 修改每个分类的父级ID
        foreach (var existingCategory in existingCategories.Where(c => c.CanDelete))
        {
            existingCategory.ParentId = category.ParentId;
        }

        // 保存更改到数据库
        _dbContext.SaveChanges();
        return true;
    }

    /// <summary>
    /// 批量删除收支分类
    /// </summary>
    /// <param name="categoryIds">分类Id集合</param>
    /// <returns></returns>
    public bool Delete(List<long> categoryIds)
    {
        if (categoryIds == null || !categoryIds.Any())
        {
            throw new BusinessException("分类ID列表不能为空");
        }

        // 查询要删除的分类
        var categoriesToDelete = QueryByIds(categoryIds);
        if (categoriesToDelete == null || !categoriesToDelete.Any())
        {
            throw new NotFoundException("未找到指定的收支分类");
        }

        // 检查是否存在不能删除的分类
        var cannotDelete = categoriesToDelete.Where(c => !c.CanDelete).ToList();
        if (cannotDelete.Any())
        {
            var names = string.Join("，", cannotDelete.Select(c => c.Name));
            // 抛出业务异常，提示不能删除的分类名称
            throw new BusinessException($"以下分类不能删除：{names}");
        }

        // 执行删除
        foreach (var category in categoriesToDelete)
        {
            SettingCommProperty.Delete(category);
        }

        // 保存更改到数据库
        _dbContext.SaveChanges();
        return true;
    }

    /// <summary>
    /// 根据id列表批量查询收支分类
    /// </summary>
    /// <param name="ids">分类ID列表</param>
    /// <returns>返回收支分类列表</returns>
    private List<TransactionCategory> QueryByIds(List<long>? ids)
    {
        if (ids == null || !ids.Any())
        {
            return new List<TransactionCategory>();
        }

        // 查询指定ID列表的收支分类
        return _dbContext.TransactionCategories
            .Where(c => ids.Contains(c.Id) && c.IsDeleted == false).ToList();
    }

    /// <summary>
    /// 根据id查询分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>返回收支分类</returns>
    private TransactionCategory? QueryById(long id)
    {
        return _dbContext.TransactionCategories
            .FirstOrDefault(c => c.Id == id && c.IsDeleted == false);
    }
}