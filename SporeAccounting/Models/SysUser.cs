﻿using SporeAccounting.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models;

/// <summary>
/// 用户表自定义属性类
/// </summary>
[Table(name: "SysUser")]
public class SysUser : BaseModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    [Required]
    public string UserName { get; set; }

    /// <summary>
    /// 加密后的密码
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// 加密用的盐
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [Required]
    public string Salt { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [Column(TypeName = "nvarchar(11)")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// 是否可以删除
    /// </summary>
    [Column(TypeName = "tinyint(1)")]
    [Required]
    public bool CanDelete { get; set; } = true;

    /// <summary>
    /// 角色id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("FK_SysUser_RoleId")]
    [Required]
    public string RoleId { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public SysRole Role { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public Config Config { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public ICollection<IncomeExpenditureRecord> IncomeExpenditureRecords { get; set; } =
        new List<IncomeExpenditureRecord>();

    /// <summary>
    /// 导航属性
    /// </summary>
    public ICollection<AccountBook> AccountBooks { get; set; } = new List<AccountBook>();
}