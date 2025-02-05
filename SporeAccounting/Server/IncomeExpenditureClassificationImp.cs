using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 收支分类数据库操作实现
/// </summary>
public class IncomeExpenditureClassificationImp : IIncomeExpenditureClassificationServer
{
    private readonly SporeAccountingDBContext _sporeAccountingDbContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sporeAccountingDbContext"></param>
    public IncomeExpenditureClassificationImp(SporeAccountingDBContext sporeAccountingDbContext)
    {
        _sporeAccountingDbContext = sporeAccountingDbContext;
    }

    /// <summary>
    /// 新增收支分类
    /// </summary>
    /// <param name="classification"></param>
    public void Add(IncomeExpenditureClassification classification)
    {
        try
        {
            _sporeAccountingDbContext.IncomeExpenditureClassifications.Add(classification);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 删除收支分类（物理删除）
    /// </summary>
    /// <param name="classificationId"></param>
    public void Delete(string classificationId)
    {
        try
        {
            IncomeExpenditureClassification classification = _sporeAccountingDbContext.IncomeExpenditureClassifications
                .FirstOrDefault(p => p.Id == classificationId)!;
            _sporeAccountingDbContext.IncomeExpenditureClassifications.Remove(classification);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 修改收支分类
    /// </summary>
    /// <param name="classification"></param>
    public void Update(IncomeExpenditureClassification classification)
    {
        try
        {
            IncomeExpenditureClassification dbClassification = _sporeAccountingDbContext
                .IncomeExpenditureClassifications
                .FirstOrDefault(p => p.Id == classification.Id)!;
            dbClassification.Name = classification.Name;
            dbClassification.Type = classification.Type;
            dbClassification.ParentClassificationId = classification.ParentClassificationId;
            dbClassification.UpdateDateTime = DateTime.Now;
            dbClassification.UpdateUserId = classification.UpdateUserId;
            _sporeAccountingDbContext.IncomeExpenditureClassifications.Update(dbClassification);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 根据收支类型查询
    /// </summary>
    /// <param name="type"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public IQueryable<IncomeExpenditureClassification> Query(IncomeExpenditureTypeEnmu type, string userId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureClassifications
                .Where(p => p.Type == type && p.CreateUserId == userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 根据父级分类Id查询
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    public IQueryable<IncomeExpenditureClassification> Query(string parentId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureClassifications
                .Where(p => p.ParentClassificationId == parentId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 根据分类Id查询
    /// </summary>
    /// <param name="classificationId"></param>
    /// <returns></returns>
    public IncomeExpenditureClassification QueryById(string classificationId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureClassifications
                .FirstOrDefault(p => p.Id == classificationId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 分页查询收支分类
    /// </summary>
    /// <param name="sysRoleUrlPageViewModel"></param>
    /// <returns></returns>
    public (int rowCount, int pageCount, List<IncomeExpenditureClassification> incomeExpenditureClassifications)
        GetByPage(
            IncomeExpenditureClassificationPageViewModel sysRoleUrlPageViewModel)
    {
        try
        {
            IQueryable<IncomeExpenditureClassification> incomeExpenditureClassifications =
                _sporeAccountingDbContext.IncomeExpenditureClassifications;
            if (!string.IsNullOrEmpty(sysRoleUrlPageViewModel.ClassificationName))
            {
                incomeExpenditureClassifications = incomeExpenditureClassifications
                    .Where(p => p.Name.Contains(sysRoleUrlPageViewModel.ClassificationName));
            }

            if (sysRoleUrlPageViewModel.Type != null)
            {
                incomeExpenditureClassifications = incomeExpenditureClassifications
                    .Where(p => p.Type == sysRoleUrlPageViewModel.Type);
            }

            if (!string.IsNullOrEmpty(sysRoleUrlPageViewModel.ParentClassificationId))
            {
                incomeExpenditureClassifications = incomeExpenditureClassifications
                    .Where(p => p.ParentClassificationId == sysRoleUrlPageViewModel.ParentClassificationId);
            }

            int rowCount = incomeExpenditureClassifications.Count();
            int pageCount = (int)Math.Ceiling(rowCount / (double)sysRoleUrlPageViewModel.PageSize);
            incomeExpenditureClassifications = incomeExpenditureClassifications
                .OrderBy(p => p.CreateDateTime)
                .Skip((sysRoleUrlPageViewModel.PageNumber - 1) * sysRoleUrlPageViewModel.PageSize)
                .Take(sysRoleUrlPageViewModel.PageSize);
            return (rowCount, pageCount, incomeExpenditureClassifications.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 分类名称是否存在
    /// </summary>
    /// <param name="classificationName"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsExist(string classificationName, string userId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureClassifications
                .Any(p => p.Name == classificationName && p.CreateUserId == userId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 指定id的分类是否存在
    /// </summary>
    /// <param name="classificationId"></param>
    /// <returns></returns>
    public bool IsExist(string classificationId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureClassifications
                .Any(p => p.Id == classificationId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 是否可删除
    /// </summary>
    /// <param name="classificationId"></param>
    /// <returns></returns>
    public bool CanDelete(string classificationId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureClassifications
                .FirstOrDefault(p => p.Id == classificationId).CanDelete;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 是否存在子类型
    /// </summary>
    /// <param name="classificationId"></param>
    /// <returns></returns>
    public bool HasChild(string classificationId)
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureClassifications
                .Any(p => p.ParentClassificationId == classificationId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    /// <summary>
    /// 查询父级分类
    /// </summary>
    /// <returns></returns>
    public List<IncomeExpenditureClassification> QueryParent()
    {
        try
        {
            return _sporeAccountingDbContext.IncomeExpenditureClassifications
                .Where(p => p.ParentClassificationId == null).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}