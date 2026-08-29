using Microsoft.AspNetCore.Identity;

namespace SP.IdentityService.Models.Entity;

public class SpUser:IdentityUser<long>
{
    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}