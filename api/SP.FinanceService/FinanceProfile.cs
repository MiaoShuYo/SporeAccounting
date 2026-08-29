using AutoMapper;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService;

/// <summary>
/// 财务服务映射配置
/// </summary>
public class FinanceProfile : Profile
{
    public FinanceProfile()
    {
        // TransactionCategory 映射
        CreateMap<TransactionCategory, TransactionCategoryResponse>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));
        CreateMap<TransactionCategoryAddRequest, TransactionCategory>();
        CreateMap<TransactionCategoryEditRequest, TransactionCategory>();
        
        // AccountBook 映射
        CreateMap<AccountBookAddRequest, AccountBook>();
        CreateMap<AccountBookEditeRequest, AccountBook>();
        CreateMap<AccountBook, AccountBookResponse>();

        // Accounting 映射
        CreateMap<AccountingAddRequest, Accounting>()
            .ForMember(dest => dest.BeforAmount, opt =>
                opt.MapFrom(src => src.Amount));

        CreateMap<AccountingEditRequest, Accounting>()
            .ForMember(dest => dest.BeforAmount, opt =>
                opt.MapFrom(src => src.Amount));

        CreateMap<Accounting, AccountingResponse>()
            .ForMember(dest => dest.Amount, opt =>
                opt.MapFrom(src => src.BeforAmount));

        // Budget 映射
        CreateMap<BudgetAddRequest, Budget>();
        CreateMap<BudgetEditRequest, Budget>();
        CreateMap<Budget, BudgetResponse>();
        
        // BudgetRecord 映射
        CreateMap<BudgetRecord, BudgetRecordResponse>();

        // 账本分享映射
        CreateMap<AccountBookShareAddRequest, AccountBookShare>();

        // 定期支出映射
        CreateMap<RecurringExpenseRuleAddRequest, RecurringExpenseRule>();
        CreateMap<RecurringExpenseRuleEditRequest, RecurringExpenseRule>();
        CreateMap<RecurringExpenseRule, RecurringExpenseRuleResponse>();

        // 分摊结算映射
        CreateMap<SharedExpenseSettlementAddRequest, SharedExpenseSettlement>();
        CreateMap<SharedExpenseSettlement, SharedExpenseSettlementResponse>();

        // 分摊账目映射
        CreateMap<SharedExpenseAddRequest, SharedExpense>();
        CreateMap<SharedExpenseEditRequest, SharedExpense>();
        CreateMap<SharedExpenseParticipantAddRequest, SharedExpenseParticipant>();
        CreateMap<SharedExpense, SharedExpenseResponse>();
        CreateMap<SharedExpenseParticipant, SharedExpenseParticipantResponse>();

        // 分摊提醒映射
        CreateMap<SharedExpenseReminderAddRequest, SharedExpenseReminder>();
        CreateMap<SharedExpenseReminder, SharedExpenseReminderResponse>();

        // 常用支付方式映射
        CreateMap<PaymentMethodAddRequest, PaymentMethod>();
        CreateMap<PaymentMethodEditRequest, PaymentMethod>();
        CreateMap<PaymentMethod, PaymentMethodResponse>();

        // 财务健康评分映射
        CreateMap<FinancialHealthScore, FinancialHealthScoreResponse>()
            .ForMember(dest => dest.HealthLevel, opt => opt.MapFrom(src => (int)src.HealthLevel))
            .ForMember(dest => dest.HealthLevelName, opt => opt.Ignore());
    }
}