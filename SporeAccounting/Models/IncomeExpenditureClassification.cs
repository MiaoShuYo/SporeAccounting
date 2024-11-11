﻿using System.ComponentModel.DataAnnotations;
using SporeAccounting.BaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models;

/// <summary>
/// 收支分类
/// </summary>
[Table(name: "IncomeExpenditureClassification")]
public class IncomeExpenditureClassification: BaseModel
{
    /// <summary>
    /// 类型名称
    /// </summary>
    [Required(ErrorMessage = "类型名称不能为空")]
    [MaxLength(20,ErrorMessage = "类型名称不能超过20字")]
    [Column(TypeName = "nvarchar(20)")]
    public string Name { get; set; }
    /// <summary>
    /// 收支类型
    /// </summary>
    [Required(ErrorMessage = "收支类型能为空")]
    [Column(TypeName = "int")]
    public IncomeExpenditureTypeEnmu Type { get; set; }
    /// <summary>
    /// 父级分类ID
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    public string? ParentClassificationId { get; set; }
}