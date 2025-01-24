using AutoMapper;
using SporeAccounting.Models.ViewModels;

namespace SporeAccounting.Models;

/// <summary>
/// SporeAccountingProfile
/// </summary>
public class SporeAccountingProfile : Profile
{
    /// <summary>
    /// 构造函数
    /// </summary>
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
        CreateMap<SysUser, SysUserInfoViewModel>()
            .ForMember(d => d.PhoneNumber, opt =>
                opt.MapFrom(s => s.PhoneNumber))
            .ForMember(d => d.Email, opt =>
                opt.MapFrom(s => s.Email))
            .ForMember(d => d.UserName, opt =>
                opt.MapFrom(s => s.UserName))
            .ForMember(d => d.Id, opt =>
                opt.MapFrom(s => s.Id))
            .ForMember(d => d.Configs, opt =>
                opt.MapFrom(s => s.Configs));


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

        CreateMap<SysRoleUrlAddViewModel, SysRoleUrl>();

        CreateMap<IncomeExpenditureClassification, IncomeExpenditureClassificationInfoViewModel>();
        CreateMap<IncomeExpenditureClassificationViewModel, IncomeExpenditureClassification>();
        CreateMap<IncomeExpenditureClassificationEditViewModel, IncomeExpenditureClassification>();

        CreateMap<Config, ConfigViewModel>();
        CreateMap<Config, ConfigInfoViewModel>()
            .ForMember(d => d.Id, opt =>
                opt.MapFrom(s => s.Id))
            .ForMember(d => d.Value, opt =>
                opt.MapFrom(s => s.Value))
            .ForMember(d => d.ConfigType, opt =>
                opt.MapFrom(s => s.ConfigType));

        CreateMap<AccountBookAddViewmModel, AccountBook>()
            .ForMember(d => d.Name, opt =>
                opt.MapFrom(s => s.Name));
        CreateMap<AccountBookUpdateViewModel, AccountBook>()
            .ForMember(d => d.Name, opt =>
                opt.MapFrom(s => s.Name))
            .ForMember(d => d.Remarks, opt =>
                opt.MapFrom(s => s.Remarks))
            .ForMember(d => d.Balance, opt =>
                opt.MapFrom(s => s.Balance))
            .ForMember(d => d.Id, opt =>
                opt.MapFrom(s => s.AccountBookId));
        CreateMap<AccountBook, AccountBookInfoViewModel>()
            .ForMember(d => d.AccountBookId, opt =>
                opt.MapFrom(s => s.Id))
            .ForMember(d => d.Name, opt =>
                opt.MapFrom(s => s.Name))
            .ForMember(d => d.Remarks, opt =>
                opt.MapFrom(s => s.Remarks));

        CreateMap<IncomeExpenditureRecordAddViewModel, IncomeExpenditureRecord>()
            .ForMember(d => d.BeforAmount, opt => opt
                .MapFrom(s => s.Amount))
            .ForMember(d => d.IncomeExpenditureClassificationId, opt => opt
                .MapFrom(s => s.IncomeExpenditureClassificationId))
            .ForMember(d => d.RecordDate, opt => opt
                .MapFrom(s => s.RecordDate))
            .ForMember(d => d.CurrencyId, opt => opt
                .MapFrom(s => s.CurrencyId))
            .ForMember(d => d.Remark, opt => opt
                .MapFrom(s => s.Remark))
            .ForMember(d => d.AccountBookId, opt => opt
                .MapFrom(s => s.AccountBookId));
        CreateMap<IncomeExpenditureRecordEditViewModel, IncomeExpenditureRecord>()
            .ForMember(d => d.BeforAmount, opt => opt
                .MapFrom(s => s.Amount))
            .ForMember(d => d.IncomeExpenditureClassificationId, opt => opt
                .MapFrom(s => s.IncomeExpenditureClassificationId))
            .ForMember(d => d.RecordDate, opt => opt
                .MapFrom(s => s.RecordDate))
            .ForMember(d => d.CurrencyId, opt => opt
                .MapFrom(s => s.CurrencyId))
            .ForMember(d => d.Remark, opt => opt
                .MapFrom(s => s.Remark))
            .ForMember(d => d.AccountBookId, opt => opt
                .MapFrom(s => s.AccountBookId))
            .ForMember(d => d.Id, opt => opt
                .MapFrom(s => s.IncomeExpenditureRecordId));
        CreateMap<IncomeExpenditureRecord, IncomeExpenditureRecordInfoViewModel>()
            .ForMember(d => d.BeforAmount, opt => opt
                .MapFrom(s => s.BeforAmount))
            .ForMember(d => d.AfterAmount, opt => opt
                .MapFrom(s => s.AfterAmount))
            .ForMember(d => d.IncomeExpenditureClassificationId, opt => opt
                .MapFrom(s => s.IncomeExpenditureClassificationId))
            .ForMember(d => d.RecordDate, opt => opt
                .MapFrom(s => s.RecordDate))
            .ForMember(d => d.CurrencyId, opt => opt
                .MapFrom(s => s.CurrencyId))
            .ForMember(d => d.Remark, opt => opt
                .MapFrom(s => s.Remark))
            .ForMember(d => d.AccountBookId, opt => opt
                .MapFrom(s => s.AccountBookId))
            .ForMember(d => d.IncomeExpenditureRecordId, opt => opt
                .MapFrom(s => s.Id))
            .ForMember(d => d.IncomeExpenditureClassificationName, opt => opt
                .MapFrom(s => s.IncomeExpenditureClassification.Name))
            .ForMember(d => d.AccountBookName, opt => opt
                .MapFrom(s => s.AccountBook.Name))
            .ForMember(d => d.CurrencyName, opt => opt
                .MapFrom(s => s.Currency.Name));

        CreateMap<BudgetAddViewModel, Budget>()
            .ForMember(d => d.IncomeExpenditureClassificationId, opt => opt
                .MapFrom(s => s.ClassificationId))
            .ForMember(d => d.Amount, opt => opt
                .MapFrom(s => s.Amount))
            .ForMember(d => d.Period, opt => opt
                .MapFrom(s => s.Period))
            .ForMember(d => d.Remark, opt => opt
                .MapFrom(s => s.Remark))
            .ForMember(d => d.StartTime, opt => opt
                .MapFrom(s => s.StartTime))
            .ForMember(d => d.EndTime, opt => opt
                .MapFrom(s => s.EndTime));
        CreateMap<BudgetUpdateViewModel, Budget>()
            .ForMember(d=>d.Amount, opt=>opt
                .MapFrom(s=>s.Amount))
            .ForMember(d=>d.StartTime, opt=>opt
                .MapFrom(s=>s.StartTime))
            .ForMember(d=>d.Remark, opt=>opt
                .MapFrom(s=>s.Remark))
            .ForMember(d=>d.Period, opt=>opt
                .MapFrom(s=>s.Period))
            .ForMember(d=>d.Remaining, opt=>opt
                .MapFrom(s=>s.Remaining))
            .ForMember(d=>d.EndTime, opt=>opt
                .MapFrom(s=>s.EndTime))
            .ForMember(d=>d.Id, opt=>opt
                .MapFrom(s=>s.Id));
        CreateMap<Budget, BudgetViewModel>()
            .ForMember(d => d.Amount, opt => opt
                .MapFrom(s => s.Amount))
            .ForMember(d => d.Id, opt => opt
                .MapFrom(s => s.Id))
            .ForMember(d => d.Remark, opt => opt
                .MapFrom(s => s.Remark))
            .ForMember(d => d.StartTime, opt => opt
                .MapFrom(s => s.StartTime))
            .ForMember(d => d.Period, opt => opt
                .MapFrom(s => s.Period))
            .ForMember(d => d.ClassificationName, opt => opt
                .MapFrom(s => s.Classification.Name))
            .ForMember(d => d.EndTime, opt => opt
                .MapFrom(s => s.EndTime))
            .ForMember(d=>d.Remaining, opt=>opt
                .MapFrom(s=>s.Remaining));

        CreateMap<Report, ReportResponseViewModel>()
            .ForMember(d => d.Amount, opt => opt
                .MapFrom(s => s.Amount))
            .ForMember(d => d.Month, opt => opt
                .MapFrom(s => s.Month))
            .ForMember(d => d.Year, opt => opt
                .MapFrom(s => s.Year));

    }
}