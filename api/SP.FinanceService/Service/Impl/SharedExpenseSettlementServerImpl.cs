using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
/// ��̯�������ʵ��
/// </summary>
public class SharedExpenseSettlementServerImpl : ISharedExpenseSettlementServer
{
    private readonly FinanceServiceDbContext _dbContext;
    private readonly IMapper _autoMapper;
    private readonly IAccountingServer _accountingServer;
    private readonly ContextSession _contextSession;

    public SharedExpenseSettlementServerImpl(
        FinanceServiceDbContext dbContext,
        IMapper autoMapper,
        IAccountingServer accountingServer,
        ContextSession contextSession)
    {
        _dbContext = dbContext;
        _autoMapper = autoMapper;
        _accountingServer = accountingServer;
        _contextSession = contextSession;
    }

    /// <summary>
    /// ���������¼
    /// </summary>
    /// <param name="request">��������</param>
    /// <returns>�����¼Id</returns>
    public long Add(SharedExpenseSettlementAddRequest request)
    {
        long currentUserId = _contextSession.UserId;
        if (request.ParticipantId != currentUserId)
        {
            throw new ForbiddenException("仅允许当前用户提交自己的结算记录");
        }

        var sharedExpense = _dbContext.SharedExpenses
            .FirstOrDefault(x => x.Id == request.SharedExpenseId && !x.IsDeleted);

        if (sharedExpense == null)
        {
            throw new NotFoundException($"��̯��Ŀ�����ڣ�ID: {request.SharedExpenseId}");
        }

        var participant = _dbContext.SharedExpenseParticipants
            .FirstOrDefault(x => x.SharedExpenseId == request.SharedExpenseId
                                 && x.ParticipantId == request.ParticipantId
                                 && !x.IsDeleted);

        if (participant == null)
        {
            throw new NotFoundException($"��̯�����߲����ڣ�ParticipantId: {request.ParticipantId}");
        }

        long accountingId = CreateIncomeAccounting(sharedExpense, request);

        var entity = _autoMapper.Map<SharedExpenseSettlement>(request);
        entity.ReceiverId = sharedExpense.PayerId;
        entity.AccountingId = accountingId;
        SettingCommProperty.Create(entity);
        _dbContext.SharedExpenseSettlements.Add(entity);

        participant.IsPaid = true;
        participant.SettlementDate = request.SettlementDate;
        participant.AccountingId = accountingId;
        if (!string.IsNullOrWhiteSpace(request.ProofUrl))
        {
            participant.ProofUrl = request.ProofUrl;
        }

        if (!string.IsNullOrWhiteSpace(request.Remark))
        {
            participant.Remark = request.Remark;
        }

        SettingCommProperty.Edit(participant);
        _dbContext.SharedExpenseParticipants.Update(participant);

        UpdateSharedExpenseStatus(sharedExpense);

        _dbContext.SaveChanges();
        return entity.Id;
    }

    private long CreateIncomeAccounting(SharedExpense sharedExpense, SharedExpenseSettlementAddRequest request)
    {
        var accountingRequest = new AccountingAddRequest
        {
            Amount = request.PaymentAmount,
            TransactionCategoryId = sharedExpense.TransactionCategoryId,
            AccountBookId = sharedExpense.AccountBookId,
            RecordDate = request.SettlementDate,
            CurrencyId = sharedExpense.CurrencyId,
            Remark = string.IsNullOrWhiteSpace(request.Remark)
                ? $"��̯�տ{sharedExpense.Title}"
                : request.Remark
        };

        return _accountingServer.Add(sharedExpense.AccountBookId, accountingRequest)
            .GetAwaiter()
            .GetResult();
    }

    private void UpdateSharedExpenseStatus(SharedExpense sharedExpense)
    {
        var participants = _dbContext.SharedExpenseParticipants
            .Where(x => x.SharedExpenseId == sharedExpense.Id && !x.IsDeleted)
            .ToList();

        if (participants.Count == 0)
        {
            return;
        }

        bool allPaid = participants.All(x => x.IsPaid);
        bool anyPaid = participants.Any(x => x.IsPaid);

        sharedExpense.Status = allPaid
            ? SharedExpenseStatusEnum.All
            : anyPaid
                ? SharedExpenseStatusEnum.PartialPayment
                : SharedExpenseStatusEnum.Unpaid;

        SettingCommProperty.Edit(sharedExpense);
        _dbContext.SharedExpenses.Update(sharedExpense);
    }

    /// <summary>
    /// ��ȡ��������
    /// </summary>
    /// <param name="id">�����¼Id</param>
    /// <returns>��������</returns>
    public SharedExpenseSettlementResponse QueryById(long id)
    {
        long currentUserId = _contextSession.UserId;
        var entity = _dbContext.SharedExpenseSettlements
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id && !x.IsDeleted
                && (x.ParticipantId == currentUserId || x.ReceiverId == currentUserId));

        if (entity == null)
        {
            throw new NotFoundException($"�����¼�����ڣ�ID: {id}");
        }

        return _autoMapper.Map<SharedExpenseSettlementResponse>(entity);
    }

    /// <summary>
    /// ���ݷ�̯��ĿId��ѯ�����¼�б�
    /// </summary>
    /// <param name="sharedExpenseId">��̯��ĿId</param>
    /// <returns>�����¼�б�</returns>
    public List<SharedExpenseSettlementResponse> QueryBySharedExpenseId(long sharedExpenseId)
    {
        long currentUserId = _contextSession.UserId;
        bool hasAccess = _dbContext.SharedExpenses.Any(x =>
                             !x.IsDeleted && x.Id == sharedExpenseId && x.PayerId == currentUserId)
                         || _dbContext.SharedExpenseParticipants.Any(x =>
                             !x.IsDeleted && x.SharedExpenseId == sharedExpenseId && x.ParticipantId == currentUserId);
        if (!hasAccess)
        {
            throw new ForbiddenException("禁止查看其他用户的分摊结算");
        }

        var entities = _dbContext.SharedExpenseSettlements
            .AsNoTracking()
            .Where(x => x.SharedExpenseId == sharedExpenseId && !x.IsDeleted)
            .OrderByDescending(x => x.SettlementDate)
            .ToList();

        return _autoMapper.Map<List<SharedExpenseSettlementResponse>>(entities);
    }
}
