using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 账本分享服务接口
/// </summary>
public interface IAccountBookShareServer
{
    /// <summary>
    /// 分享账本
    /// </summary>
    /// <param name="request"></param>
    System.Threading.Tasks.Task Share(AccountBookShareAddRequest request);

    PageResponse<AccountBookShareResponse> Page(AccountBookSharePageRequest request);

    /// <summary>
    /// 分页获取分享给我的账本
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    PageResponse<AccountBookShareResponse> PageSharesToMe(AccountBookSharePageRequest request);

    /// <summary>
    /// 撤销账本分享
    /// </summary>
    /// <param name="request"></param>
    System.Threading.Tasks.Task Revoke(AccountBookRevokeSharingRequest request);

    /// <summary>
    /// 编辑账本分享
    /// </summary>
    /// <param name="request"></param>
    System.Threading.Tasks.Task Edit(AccountBookShareEditRequest request);

    /// <summary>
    /// 根据账本id集合查询账本权限
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Dictionary<long, PermissionTypeEnum> GetPermission(List<long> ids);
}