using AutoMapper;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 常用支付方式服务实现类
/// </summary>
public class PaymentMethodServerImpl : IPaymentMethodServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _auMapper;

    /// <summary>
    /// 上下文会话
    /// </summary>
    private readonly ContextSession _contextSession;

    /// <summary>
    /// 常用支付方式服务实现类构造函数
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="auMapper">自动映射器</param>
    /// <param name="contextSession">上下文会话</param>
    public PaymentMethodServerImpl(FinanceServiceDbContext dbContext, IMapper auMapper,
        ContextSession contextSession)
    {
        _dbContext = dbContext;
        _auMapper = auMapper;
        _contextSession = contextSession;
    }

    /// <summary>
    /// 添加支付方式
    /// </summary>
    /// <param name="request">支付方式添加请求</param>
    /// <returns>支付方式ID</returns>
    public long Add(PaymentMethodAddRequest request)
    {
        ValidateElectronicPaymentType(request.Type, request.ElectronicPaymentType);

        long userId = _contextSession.UserId;

        if (request.IsDefault)
        {
            ClearDefault(userId);
        }

        var entity = _auMapper.Map<PaymentMethod>(request);
        if (request.Type != PaymentMethodTypeEnum.ElectronicPayment)
        {
            entity.ElectronicPaymentType = null;
        }

        SettingCommProperty.Create(entity);
        _dbContext.PaymentMethods.Add(entity);
        _dbContext.SaveChanges();
        return entity.Id;
    }

    /// <summary>
    /// 删除支付方式
    /// </summary>
    /// <param name="id">支付方式ID</param>
    public void Delete(long id)
    {
        long userId = _contextSession.UserId;
        var entity = _dbContext.PaymentMethods
            .FirstOrDefault(p => p.Id == id && p.CreateUserId == userId && !p.IsDeleted);

        if (entity == null)
        {
            throw new NotFoundException("支付方式不存在", id);
        }

        if (entity.IsDefault)
        {
            throw new BusinessException("默认支付方式不能删除，请先更换默认支付方式");
        }

        SettingCommProperty.Delete(entity);
        _dbContext.PaymentMethods.Update(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 修改支付方式
    /// </summary>
    /// <param name="request">支付方式修改请求</param>
    public void Edit(PaymentMethodEditRequest request)
    {
        ValidateElectronicPaymentType(request.Type, request.ElectronicPaymentType);

        long userId = _contextSession.UserId;
        var entity = _dbContext.PaymentMethods
            .FirstOrDefault(p => p.Id == request.Id && p.CreateUserId == userId && !p.IsDeleted);

        if (entity == null)
        {
            throw new NotFoundException("支付方式不存在", request.Id);
        }

        entity.Name = request.Name;
        entity.Type = request.Type;
        entity.ElectronicPaymentType = request.Type == PaymentMethodTypeEnum.ElectronicPayment
            ? request.ElectronicPaymentType
            : null;
        entity.Remark = request.Remark;

        SettingCommProperty.Edit(entity);
        _dbContext.PaymentMethods.Update(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 设置默认支付方式
    /// </summary>
    /// <param name="id">支付方式ID</param>
    public void SetDefault(long id)
    {
        long userId = _contextSession.UserId;
        var entity = _dbContext.PaymentMethods
            .FirstOrDefault(p => p.Id == id && p.CreateUserId == userId && !p.IsDeleted);

        if (entity == null)
        {
            throw new NotFoundException("支付方式不存在", id);
        }

        if (entity.IsDefault)
        {
            return;
        }

        ClearDefault(userId);

        entity.IsDefault = true;
        SettingCommProperty.Edit(entity);
        _dbContext.PaymentMethods.Update(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 查询当前用户所有支付方式
    /// </summary>
    /// <returns>支付方式列表</returns>
    public List<PaymentMethodResponse> QueryAll()
    {
        long userId = _contextSession.UserId;
        var list = _dbContext.PaymentMethods
            .Where(p => p.CreateUserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.CreateDateTime)
            .ToList();

        return _auMapper.Map<List<PaymentMethodResponse>>(list);
    }

    /// <summary>
    /// 清除当前用户所有默认支付方式标记
    /// </summary>
    /// <param name="userId">用户ID</param>
    private void ClearDefault(long userId)
    {
        var currentDefault = _dbContext.PaymentMethods
            .Where(p => p.CreateUserId == userId && p.IsDefault && !p.IsDeleted)
            .ToList();

        foreach (var item in currentDefault)
        {
            item.IsDefault = false;
            SettingCommProperty.Edit(item);
        }

        if (currentDefault.Count > 0)
        {
            _dbContext.PaymentMethods.UpdateRange(currentDefault);
        }
    }

    /// <summary>
    /// 校验电子支付子类型
    /// </summary>
    /// <param name="type">支付方式类型</param>
    /// <param name="electronicPaymentType">电子支付子类型</param>
    private static void ValidateElectronicPaymentType(PaymentMethodTypeEnum type,
        ElectronicPaymentTypeEnum? electronicPaymentType)
    {
        if (type == PaymentMethodTypeEnum.ElectronicPayment && electronicPaymentType == null)
        {
            throw new BusinessException("电子支付方式必须指定子类型");
        }
    }
}
