using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// ��̯��Ŀ����ӿ�
/// </summary>
public interface ISharedExpenseServer
{
    /// <summary>
    /// ������̯��Ŀ
    /// </summary>
    /// <param name="request">��̯��Ŀ����</param>
    /// <returns>��̯��ĿId</returns>
    System.Threading.Tasks.Task<long> Add(SharedExpenseAddRequest request);

    /// <summary>
    /// ��ȡ��̯��Ŀ����
    /// </summary>
    /// <param name="id">��̯��ĿId</param>
    /// <returns>��̯��Ŀ����</returns>
    SharedExpenseResponse QueryById(long id);

    /// <summary>
    /// �޸ķ�̯��Ŀ
    /// </summary>
    /// <param name="request">��̯��Ŀ�༭����</param>
    System.Threading.Tasks.Task Edit(SharedExpenseEditRequest request);

    /// <summary>
    /// ɾ����̯��Ŀ
    /// </summary>
    /// <param name="id">��̯��ĿId</param>
    System.Threading.Tasks.Task Delete(long id);

    /// <summary>
    /// �����˱�Id��ȡ��̯��Ŀ�б�
    /// </summary>
    /// <param name="accountBookId">�˱�Id</param>
    /// <returns>��̯��Ŀ�б�</returns>
    List<SharedExpenseResponse> QueryByAccountBookId(long accountBookId);
}
