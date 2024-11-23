using AutoMapper;
using SporeAccounting.Models.ViewModels;

namespace SporeAccounting.Models;

public class SporeAccountingProfile : Profile
{
    public SporeAccountingProfile()
    {
        CreateMap<SysUserViewModel, SysUser>();
        CreateMap<SysUser, SysUserQueryViewModel>()
            .ForMember(d => d.Id, opt =>
                opt.MapFrom(s => s.Id))
            .ForMember(d => d.UserName, opt =>
                opt.MapFrom(s => s.UserName))
            .ForMember(d => d.CreateDateTime, opt =>
                opt.MapFrom(s => s.CreateDateTime))
            .ForMember(d => d.Email, opt =>
                opt.MapFrom(s => s.Email))
            .ForMember(d => d.PhoneNumber, opt =>
                opt.MapFrom(s => s.PhoneNumber));

        CreateMap<SysRoleViewModel, SysRole>()
            .ForMember(d => d.RoleName, opt =>
                opt.MapFrom(s => s.RoleName));
        CreateMap<SysRoleEditViewModel, SysRole>()
            .ForMember(d => d.RoleName, opt =>
                opt.MapFrom(s => s.RoleName));
        CreateMap<SysRole, SysRoleQueryViewModel>()
            .ForMember(d => d.RoleName, opt =>
                opt.MapFrom(s => s.RoleName))
            .ForMember(d => d.RoleId, opt =>
                opt.MapFrom(s => s.Id));

        CreateMap<SysUrlViewModel, SysUrl>();
        CreateMap<SysUrlEditViewModel, SysUrl>();
        CreateMap<SysUrl, SysUrlQueryViewModel>()
            .ForMember(d => d.Id, opt =>
                opt.MapFrom(s => s.Id))
            .ForMember(d => d.Url, opt =>
                opt.MapFrom(s => s.Url));

        CreateMap<SysRoleUrlViewModel, SysRoleUrl>();

        CreateMap<IncomeExpenditureClassification, IncomeExpenditureClassificationInfoViewModel>();
        CreateMap<IncomeExpenditureClassificationViewModel, IncomeExpenditureClassification>();
        CreateMap<IncomeExpenditureClassificationEditViewModel, IncomeExpenditureClassification>();

        CreateMap<Config, ConfigViewModel>();

        CreateMap<AccountBookAddViewmModel, AccountBook>()
            .ForMember(d => d.Name, opt =>
                opt.MapFrom(s => s.Name));
        CreateMap<AccountBook, AccountBookInfoViewModel>()
            .ForMember(d => d.AccountBookId, opt =>
                opt.MapFrom(s => s.Id))
            .ForMember(d => d.Name, opt =>
                opt.MapFrom(s => s.Name))
            .ForMember(d => d.Remarks, opt =>
                opt.MapFrom(s => s.Remarks))
            .ForMember(d => d.Balance, opt =>
                opt.MapFrom(s => s.Balance));
    }
}