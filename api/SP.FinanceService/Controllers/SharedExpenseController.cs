using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// ��̯��Ŀ�ӿ�
/// </summary>
[Route("/api/shared-expenses")]
[ApiController]
public class SharedExpenseController : ControllerBase
{
    private readonly ISharedExpenseServer _sharedExpenseServer;

    public SharedExpenseController(ISharedExpenseServer sharedExpenseServer)
    {
        _sharedExpenseServer = sharedExpenseServer;
    }

    /// <summary>
    /// ������̯��Ŀ
    /// </summary>
    /// <param name="request">��̯��Ŀ����</param>
    /// <returns>��̯��ĿId</returns>
    [HttpPost]
    public async System.Threading.Tasks.Task<ActionResult<long>> CreateSharedExpense([FromBody] SharedExpenseAddRequest request)
    {
        long id = await _sharedExpenseServer.Add(request);
        return Ok(id);
    }

    /// <summary>
    /// ��ȡ��̯��Ŀ����
    /// </summary>
    /// <param name="id">��̯��ĿId</param>
    /// <returns>��̯��Ŀ����</returns>
    [HttpGet("{id}")]
    public ActionResult<SharedExpenseResponse> GetSharedExpense([FromRoute] long id)
    {
        SharedExpenseResponse response = _sharedExpenseServer.QueryById(id);
        return Ok(response);
    }

    /// <summary>
    /// ���·�̯��Ŀ
    /// </summary>
    /// <param name="id">��̯��ĿId</param>
    /// <param name="request">��̯��Ŀ�༭����</param>
    /// <returns>�޸Ľ��</returns>
    [HttpPut("{id}")]
    public async System.Threading.Tasks.Task<ActionResult<bool>> UpdateSharedExpense([FromRoute] long id, [FromBody] SharedExpenseEditRequest request)
    {
        if (request == null || request.Id <= 0)
        {
            return BadRequest("Invalid shared expense data.");
        }

        if (id != request.Id)
        {
            return BadRequest("Route id does not match request.Id.");
        }

        await _sharedExpenseServer.Edit(request);
        return Ok(true);
    }

    /// <summary>
    /// ɾ����̯��Ŀ
    /// </summary>
    /// <param name="id">��̯��ĿId</param>
    /// <returns>ɾ�����</returns>
    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task<ActionResult<bool>> DeleteSharedExpense([FromRoute] long id)
    {
        await _sharedExpenseServer.Delete(id);
        return Ok(true);
    }

    /// <summary>
    /// �����˱�Id��ȡ��̯��Ŀ�б�
    /// </summary>
    /// <param name="accountBookId">�˱�Id</param>
    /// <returns>��̯��Ŀ�б�</returns>
    [HttpGet]
    public ActionResult<List<SharedExpenseResponse>> GetSharedExpenses([FromQuery] long accountBookId)
    {
        List<SharedExpenseResponse> responses = _sharedExpenseServer.QueryByAccountBookId(accountBookId);
        return Ok(responses);
    }
}
