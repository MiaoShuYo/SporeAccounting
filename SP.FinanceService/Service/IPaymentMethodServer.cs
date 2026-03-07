using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 常用支付方式服务接口
/// </summary>
public interface IPaymentMethodServer
{
    /// <summary>
    /// 添加支付方式
    /// </summary>
    /// <param name="request">支付方式添加请求</param>
    /// <returns>支付方式ID</returns>
    long Add(PaymentMethodAddRequest request);

    /// <summary>
    /// 删除支付方式
    /// </summary>
    /// <param name="id">支付方式ID</param>
    void Delete(long id);

    /// <summary>
    /// 修改支付方式
    /// </summary>
    /// <param name="request">支付方式修改请求</param>
    void Edit(PaymentMethodEditRequest request);

    /// <summary>
    /// 设置默认支付方式
    /// </summary>
    /// <param name="id">支付方式ID</param>
    void SetDefault(long id);

    /// <summary>
    /// 查询当前用户所有支付方式
    /// </summary>
    /// <returns>支付方式列表</returns>
    List<PaymentMethodResponse> QueryAll();
}
