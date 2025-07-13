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
        CreateMap<TransactionCategory, TransactionCategoryResponse>()
            .ForMember(dest => dest.Id,
                opt =>
                    opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name,
                opt =>
                    opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Type,
                opt =>
                    opt.MapFrom(src => (int)src.Type));

        CreateMap<AccountBookAddRequest, AccountBook>()
            .ForMember(dest => dest.Name,
                opt =>
                    opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Remarks,
                opt =>
                    opt.MapFrom(src => src.Remarks));
        CreateMap<AccountBookEditeRequest, AccountBook>()
            .ForMember(dest => dest.Id,
                opt =>
                    opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name,
                opt =>
                    opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Remarks,
                opt =>
                    opt.MapFrom(src => src.Remarks));
        CreateMap<AccountBook, AccountBookResponse>()
            .ForMember(dest => dest.Id,
                opt =>
                    opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name,
                opt =>
                    opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Remarks,
                opt =>
                    opt.MapFrom(src => src.Remarks));

        CreateMap<AccountingAddRequest, Accounting>()
            .ForMember(dest => dest.AccountBookId,
                opt =>
                    opt.MapFrom(src => src.AccountBookId))
            .ForMember(dest => dest.TransactionCategoryId,
                opt =>
                    opt.MapFrom(src => src.TransactionCategoryId))
            .ForMember(dest => dest.BeforAmount,
                opt =>
                    opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.RecordDate,
                opt =>
                    opt.MapFrom(src => src.RecordDate))
            .ForMember(dest => dest.Remark,
                opt =>
                    opt.MapFrom(src => src.Remark));
        CreateMap<AccountingEditRequest, Accounting>()
            .ForMember(dest => dest.Id,
                opt =>
                    opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AccountBookId,
                opt =>
                    opt.MapFrom(src => src.AccountBookId))
            .ForMember(dest => dest.TransactionCategoryId,
                opt =>
                    opt.MapFrom(src => src.TransactionCategoryId))
            .ForMember(dest => dest.BeforAmount,
                opt =>
                    opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.RecordDate,
                opt =>
                    opt.MapFrom(src => src.RecordDate))
            .ForMember(dest => dest.Remark,
                opt =>
                    opt.MapFrom(src => src.Remark));
        CreateMap<Accounting, AccountingResponse>()
            .ForMember(dest => dest.Id,
                opt =>
                    opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TransactionCategoryId,
                opt =>
                    opt.MapFrom(src => src.TransactionCategoryId))
            .ForMember(dest => dest.Amount,
                opt =>
                    opt.MapFrom(src => src.AfterAmount))
            .ForMember(dest => dest.CurrencyId,
                opt =>
                    opt.MapFrom(src => src.CurrencyId))
            .ForMember(dest => dest.RecordDate,
                opt =>
                    opt.MapFrom(src => src.RecordDate))
            .ForMember(dest => dest.Remark,
                opt =>
                    opt.MapFrom(src => src.Remark));
    }
}