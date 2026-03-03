using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 分摊账目服务接口
/// </summary>
public interface ISharedExpenseServer
{
    /// <summary>
    /// 创建分摊账目
    /// </summary>
    /// <param name="request">分摊账目请求</param>
    /// <returns>分摊账目Id</returns>
    long Add(SharedExpenseAddRequest request);

    /// <summary>
    /// 获取分摊账目详情
    /// </summary>
    /// <param name="id">分摊账目Id</param>
    /// <returns>分摊账目详情</returns>
    SharedExpenseResponse QueryById(long id);

    /// <summary>
    /// 修改分摊账目
    /// </summary>
    /// <param name="request">分摊账目编辑请求</param>
    void Edit(SharedExpenseEditRequest request);

    /// <summary>
    /// 删除分摊账目
    /// </summary>
    /// <param name="id">分摊账目Id</param>
    void Delete(long id);

    /// <summary>
    /// 根据账本Id获取分摊账目列表
    /// </summary>
    /// <param name="accountBookId">账本Id</param>
    /// <returns>分摊账目列表</returns>
    List<SharedExpenseResponse> QueryByAccountBookId(long accountBookId);
}
