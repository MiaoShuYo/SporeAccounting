﻿using SporeAccounting.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models;

/// <summary>
/// 角色
/// </summary>
[Table(name: "SysRole")]
public class SysRole : BaseModel
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    [Required]
    public string RoleName { get; set; }
    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUser User { get; set; }
}