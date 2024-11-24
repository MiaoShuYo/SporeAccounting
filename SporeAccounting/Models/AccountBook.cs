﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;
using SporeAccounting.BaseModels;

namespace SporeAccounting.Models;

/// <summary>
/// 账簿
/// </summary>
[Table(name: "AccountBook")]
public class AccountBook : BaseModel
{
    /// <summary>
    /// 账簿名称
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("FK_AccountBook_SysUser")]
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUser User { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public ICollection<IncomeExpenditureRecord> IncomeExpenditureRecords { get; set; } =
        new List<IncomeExpenditureRecord>();
}