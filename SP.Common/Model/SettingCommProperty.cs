using SP.Common.ExceptionHandling.Exceptions;
using Microsoft.AspNetCore.Http;

namespace SP.Common.Model;

/// <summary>
/// 通用设置属性类
/// </summary>
public class SettingCommProperty
{
    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="model"></param>
    public static void Delete(BaseModel model)
    {
        if (model is null)
        {
            throw new BusinessException("Model cannot be null");
        }

        model.IsDeleted = true;
        model.UpdateDateTime = DateTime.Now;
        model.UpdateUserId = GetCurrentUserId();
    }

    /// <summary>
    /// 新建
    /// </summary>
    /// <param name="model"></param>
    public static void Create(BaseModel model)
    {
        if (model is null)
        {
            throw new BusinessException("Model cannot be null");
        }

        model.Id = Snow.GetId();
        model.CreateDateTime = DateTime.Now;
        model.CreateUserId = GetCurrentUserId();
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="model"></param>
    public static void Edit(BaseModel model)
    {
        if (model is null)
        {
            throw new BusinessException("Model cannot be null");
        }
        model.UpdateDateTime = DateTime.Now;
        model.UpdateUserId = GetCurrentUserId();
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    /// <returns></returns>
    private static long GetCurrentUserId()
    {
        var httpContextAccessor = (IHttpContextAccessor)AppDomain.CurrentDomain.GetData("HttpContextAccessor");
        var userIdClaim = httpContextAccessor?.HttpContext?.User?.FindFirst("UserId");
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return 0;
    }
}