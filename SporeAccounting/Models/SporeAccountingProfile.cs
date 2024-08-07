using AutoMapper;
using SporeAccounting.Models.ViewModels;

namespace SporeAccounting.Models;

public class SporeAccountingProfile:Profile
{
    public SporeAccountingProfile()
    {
        CreateMap<SysUserViewModel, SysUser>();
    }
}