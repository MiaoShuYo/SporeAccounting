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
    }
}