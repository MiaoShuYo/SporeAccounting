using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 常用支付方式控制器
/// </summary>
[Route("api/payment-methods")]
[ApiController]
public class PaymentMethodController : ControllerBase
{
    /// <summary>
    /// 常用支付方式服务
    /// </summary>
    private readonly IPaymentMethodServer _paymentMethodServer;

    /// <summary>
    /// 常用支付方式控制器构造函数
    /// </summary>
    /// <param name="paymentMethodServer">常用支付方式服务</param>
    public PaymentMethodController(IPaymentMethodServer paymentMethodServer)
    {
        _paymentMethodServer = paymentMethodServer;
    }

    /// <summary>
    /// 添加支付方式
    /// </summary>
    /// <param name="request">支付方式添加请求</param>
    /// <returns>支付方式ID</returns>
    [HttpPost]
    public ActionResult<long> CreatePaymentMethod([FromBody] PaymentMethodAddRequest request)
    {
        long id = _paymentMethodServer.Add(request);
        return Ok(id);
    }

    /// <summary>
    /// 删除支付方式
    /// </summary>
    /// <param name="id">支付方式ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public ActionResult<bool> DeletePaymentMethod([FromRoute] long id)
    {
        _paymentMethodServer.Delete(id);
        return Ok(true);
    }

    /// <summary>
    /// 修改支付方式
    /// </summary>
    /// <param name="id">支付方式ID</param>
    /// <param name="request">支付方式修改请求</param>
    /// <returns>修改结果</returns>
    [HttpPut("{id}")]
    public ActionResult<bool> UpdatePaymentMethod([FromRoute] long id, [FromBody] PaymentMethodEditRequest request)
    {
        if (request == null || request.Id <= 0)
        {
            return BadRequest("请求数据无效");
        }

        if (id != request.Id)
        {
            return BadRequest("路由ID与请求体ID不一致");
        }

        _paymentMethodServer.Edit(request);
        return Ok(true);
    }

    /// <summary>
    /// 设置默认支付方式
    /// </summary>
    /// <param name="id">支付方式ID</param>
    /// <returns>设置结果</returns>
    [HttpPatch("{id}/default")]
    public ActionResult<bool> SetDefaultPaymentMethod([FromRoute] long id)
    {
        _paymentMethodServer.SetDefault(id);
        return Ok(true);
    }

    /// <summary>
    /// 查询当前用户所有支付方式
    /// </summary>
    /// <returns>支付方式列表</returns>
    [HttpGet]
    public ActionResult<List<PaymentMethodResponse>> GetPaymentMethods()
    {
        List<PaymentMethodResponse> list = _paymentMethodServer.QueryAll();
        return Ok(list);
    }
}
