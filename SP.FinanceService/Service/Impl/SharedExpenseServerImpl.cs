using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// ��̯��Ŀ����ʵ��
/// </summary>
public class SharedExpenseServerImpl : ISharedExpenseServer
{
    private readonly FinanceServiceDbContext _dbContext;
    private readonly IMapper _autoMapper;
    private readonly IAccountingServer _accountingServer;
    private readonly ContextSession _contextSession;

    public SharedExpenseServerImpl(
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
    /// ������̯��Ŀ
    /// </summary>
    /// <param name="request">��̯��Ŀ����</param>
    /// <returns>��̯��ĿId</returns>
    public long Add(SharedExpenseAddRequest request)
    {
        var entity = _autoMapper.Map<SharedExpense>(request);
        entity.PayerId = _contextSession.UserId;
        entity.AccountingId = CreateAccountingRecord(request);
        SettingCommProperty.Create(entity);
        _dbContext.SharedExpenses.Add(entity);
        _dbContext.SaveChanges();

        var participants = request.Participants.Select(participantRequest =>
        {
            var participant = _autoMapper.Map<SharedExpenseParticipant>(participantRequest);
            participant.SharedExpenseId = entity.Id;
            SettingCommProperty.Create(participant);
            return participant;
        }).ToList();

        if (participants.Count > 0)
        {
            _dbContext.SharedExpenseParticipants.AddRange(participants);
            _dbContext.SaveChanges();
        }

        return entity.Id;
    }

    private long CreateAccountingRecord(SharedExpenseAddRequest request)
    {
        var accountingRequest = new AccountingAddRequest
        {
            Amount = request.TotalAmount,
            TransactionCategoryId = request.TransactionCategoryId,
            AccountBookId = request.AccountBookId,
            RecordDate = request.ExpenseDate,
            CurrencyId = request.CurrencyId,
            Remark = request.Title
        };

        return _accountingServer.Add(request.AccountBookId, accountingRequest).GetAwaiter().GetResult();
    }

    /// <summary>
    /// ��ȡ��̯��Ŀ����
    /// </summary>
    /// <param name="id">��̯��ĿId</param>
    /// <returns>��̯��Ŀ����</returns>
    public SharedExpenseResponse QueryById(long id)
    {
        long currentUserId = _contextSession.UserId;
        var entity = _dbContext.SharedExpenses
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id && !x.IsDeleted &&
                                 (x.PayerId == currentUserId || _dbContext.SharedExpenseParticipants
                                     .Any(p => !p.IsDeleted && p.SharedExpenseId == x.Id && p.ParticipantId == currentUserId)));

        if (entity == null)
        {
            throw new NotFoundException($"��̯��Ŀ�����ڣ�ID: {id}");
        }

        var participants = _dbContext.SharedExpenseParticipants
            .AsNoTracking()
            .Where(x => x.SharedExpenseId == id && !x.IsDeleted)
            .ToList();

        var response = _autoMapper.Map<SharedExpenseResponse>(entity);
        response.Participants = _autoMapper.Map<List<SharedExpenseParticipantResponse>>(participants);
        return response;
    }

    /// <summary>
    /// �����˱�Id��ȡ��̯��Ŀ�б�
    /// </summary>
    /// <param name="accountBookId">�˱�Id</param>
    /// <returns>��̯��Ŀ�б�</returns>
    public List<SharedExpenseResponse> QueryByAccountBookId(long accountBookId)
    {
        long currentUserId = _contextSession.UserId;
        var entities = _dbContext.SharedExpenses
            .AsNoTracking()
            .Where(x => x.AccountBookId == accountBookId && !x.IsDeleted &&
                        (x.PayerId == currentUserId || _dbContext.SharedExpenseParticipants
                            .Any(p => !p.IsDeleted && p.SharedExpenseId == x.Id && p.ParticipantId == currentUserId)))
            .OrderByDescending(x => x.ExpenseDate)
            .ToList();

        if (entities.Count == 0)
        {
            return new List<SharedExpenseResponse>();
        }

        var sharedExpenseIds = entities.Select(x => x.Id).ToList();
        var participants = _dbContext.SharedExpenseParticipants
            .AsNoTracking()
            .Where(x => sharedExpenseIds.Contains(x.SharedExpenseId) && !x.IsDeleted)
            .ToList();

        var responses = _autoMapper.Map<List<SharedExpenseResponse>>(entities);
        var participantLookup = participants
            .GroupBy(x => x.SharedExpenseId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var response in responses)
        {
            if (participantLookup.TryGetValue(response.Id, out var participantEntities))
            {
                response.Participants = _autoMapper.Map<List<SharedExpenseParticipantResponse>>(participantEntities);
            }
        }

        return responses;
    }

    /// <summary>
    /// �޸ķ�̯��Ŀ
    /// </summary>
    /// <param name="request">��̯��Ŀ�༭����</param>
    public void Edit(SharedExpenseEditRequest request)
    {
        long currentUserId = _contextSession.UserId;
        var entity = _dbContext.SharedExpenses
            .FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted && x.PayerId == currentUserId);

        if (entity == null)
        {
            throw new NotFoundException($"��̯��Ŀ�����ڣ�ID: {request.Id}");
        }

        entity.AccountBookId = request.AccountBookId;
        entity.Title = request.Title;
        entity.TotalAmount = request.TotalAmount;
        entity.CurrencyId = request.CurrencyId;
        entity.TransactionCategoryId = request.TransactionCategoryId;
        entity.ExpenseDate = request.ExpenseDate;
        entity.SplitType = request.SplitType;
        entity.Description = request.Description;
        SettingCommProperty.Edit(entity);
        _dbContext.SharedExpenses.Update(entity);

        UpdateAccountingRecord(entity, request);
        RefreshParticipants(entity.Id, request.AccountBookId, request.Participants);

        _dbContext.SaveChanges();
    }

    /// <summary>
    /// ɾ����̯��Ŀ
    /// </summary>
    /// <param name="id">��̯��ĿId</param>
    public void Delete(long id)
    {
        long currentUserId = _contextSession.UserId;
        var entity = _dbContext.SharedExpenses
            .FirstOrDefault(x => x.Id == id && !x.IsDeleted && x.PayerId == currentUserId);

        if (entity == null)
        {
            throw new NotFoundException($"��̯��Ŀ�����ڣ�ID: {id}");
        }

        var participants = _dbContext.SharedExpenseParticipants
            .Where(x => x.SharedExpenseId == id && !x.IsDeleted)
            .ToList();

        foreach (var participant in participants)
        {
            SettingCommProperty.Delete(participant);
        }

        if (entity.AccountingId > 0)
        {
            _accountingServer.Delete(entity.AccountBookId, entity.AccountingId);
        }

        SettingCommProperty.Delete(entity);
        _dbContext.SharedExpenseParticipants.UpdateRange(participants);
        _dbContext.SharedExpenses.Update(entity);
        _dbContext.SaveChanges();
    }

    private void UpdateAccountingRecord(SharedExpense entity, SharedExpenseEditRequest request)
    {
        if (entity.AccountingId <= 0)
        {
            return;
        }

        var accountingRequest = new AccountingEditRequest
        {
            Id = entity.AccountingId,
            Amount = request.TotalAmount,
            TransactionCategoryId = request.TransactionCategoryId,
            AccountBookId = request.AccountBookId,
            RecordDate = request.ExpenseDate,
            CurrencyId = request.CurrencyId,
            Remark = request.Title
        };

        _accountingServer.Edit(request.AccountBookId, accountingRequest).GetAwaiter().GetResult();
    }

    private void RefreshParticipants(
        long sharedExpenseId,
        long accountBookId,
        List<SharedExpenseParticipantAddRequest> participants)
    {
        var existingParticipants = _dbContext.SharedExpenseParticipants
            .Where(x => x.SharedExpenseId == sharedExpenseId && !x.IsDeleted)
            .ToList();

        var requestParticipantIds = participants
            .Select(x => x.ParticipantId)
            .ToHashSet();

        foreach (var participant in existingParticipants)
        {
            if (!requestParticipantIds.Contains(participant.ParticipantId))
            {
                if (participant.AccountingId.HasValue && participant.AccountingId.Value > 0)
                {
                    _accountingServer.Delete(accountBookId, participant.AccountingId.Value);
                }

                SettingCommProperty.Delete(participant);
            }
        }

        var requestLookup = participants
            .ToDictionary(x => x.ParticipantId, x => x);

        foreach (var participant in existingParticipants)
        {
            if (participant.IsDeleted)
            {
                continue;
            }

            if (requestLookup.TryGetValue(participant.ParticipantId, out var participantRequest))
            {
                participant.ShareAmount = participantRequest.ShareAmount;
                participant.ShareRatio = participantRequest.ShareRatio;
                participant.Remark = participantRequest.Remark;
                SettingCommProperty.Edit(participant);
            }
        }

        _dbContext.SharedExpenseParticipants.UpdateRange(existingParticipants);

        var newParticipants = participants.Select(participantRequest =>
        {
            var existing = existingParticipants
                .FirstOrDefault(x => x.ParticipantId == participantRequest.ParticipantId);

            if (existing != null && !existing.IsDeleted)
            {
                return null;
            }

            var participant = _autoMapper.Map<SharedExpenseParticipant>(participantRequest);
            participant.SharedExpenseId = sharedExpenseId;
            SettingCommProperty.Create(participant);
            return participant;
        }).ToList();

        var validNewParticipants = newParticipants
            .Where(participant => participant != null)
            .ToList();

        if (validNewParticipants.Count > 0)
        {
            _dbContext.SharedExpenseParticipants.AddRange(validNewParticipants);
        }
    }
}
