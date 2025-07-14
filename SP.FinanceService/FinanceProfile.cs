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

        // AccountBook 映射
        CreateMap<AccountBookAddRequest, AccountBook>();
        CreateMap<AccountBookEditeRequest, AccountBook>();
        CreateMap<AccountBook, AccountBookResponse>();

        // Accounting 映射
        CreateMap<AccountingAddRequest, Accounting>()
            .ForMember(dest => dest.BeforAmount, opt => opt.MapFrom(src => src.Amount));
        
        CreateMap<AccountingEditRequest, Accounting>()
            .ForMember(dest => dest.BeforAmount, opt => opt.MapFrom(src => src.Amount));
        
        CreateMap<Accounting, AccountingResponse>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.AfterAmount));

        // Budget 映射
        CreateMap<BudgetAddRequest, Budget>();
        CreateMap<BudgetEditRequest, Budget>();
        CreateMap<Budget, BudgetResponse>();
    }
}