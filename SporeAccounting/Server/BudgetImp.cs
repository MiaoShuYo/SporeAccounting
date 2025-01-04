using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 预算服务
/// </summary>
public class BudgetImp : IBudgetServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly SporeAccountingDBContext _sporeAccountingDbContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sporeAccountingDbContext"></param>
    public BudgetImp(SporeAccountingDBContext sporeAccountingDbContext)
    {
        _sporeAccountingDbContext = sporeAccountingDbContext;
    }

    /// <summary>
    /// 添加预算
    /// </summary>
    /// <param name="budget"></param>
    public void Add(Budget budget)
    {
        try
        {
            _sporeAccountingDbContext.Budgets.Add(budget);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 删除预算
    /// </summary>
    /// <param name="id"></param>
    public void Delete(string id)
    {
        try
        {
            var budget = _sporeAccountingDbContext.Budgets.Find(id);
            _sporeAccountingDbContext.Budgets.Remove(budget);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 修改预算
    /// </summary>
    /// <param name="budget"></param>
    public void Update(Budget budget)
    {
        try
        {
            _sporeAccountingDbContext.Budgets.Update(budget);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 查询预算
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<Budget> Query(string userId)
    {
        try
        {
            return _sporeAccountingDbContext.Budgets.Where(b => b.UserId == userId).ToList();
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsExist(string id)
    {
        try
        {
            return _sporeAccountingDbContext.Budgets.Any(b => b.Id == id);
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 是否是当前用户的
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsYou(string id, string userId)
    {
        try
        {
            return _sporeAccountingDbContext.Budgets.Any(b => b.Id == id && b.UserId == userId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 用户是否存在该类型预算
    /// </summary>
    /// <param name="classificationId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsExistByClassificationId(string classificationId, string userId)
    {
        try
        {
            return _sporeAccountingDbContext.Budgets.Any(b =>
                b.IncomeExpenditureClassificationId == classificationId && b.UserId == userId);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}