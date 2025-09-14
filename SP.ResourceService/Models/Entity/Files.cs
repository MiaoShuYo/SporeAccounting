using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.ResourceService.Models.Entity;

/// <summary>
/// 文件信息
/// </summary>
[Table(name: "Files")]
public class Files:BaseModel
{
    /// <summary>
    /// 文件原始名称
    /// </summary>
    [Column(TypeName = "varchar(255)")]
    public string OriginalName { get; set; }

    /// <summary>
    /// 存储对象名称
    /// </summary>
    [Column(TypeName = "varchar(255)")]
    public string ObjectName { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    [Column(TypeName = "varchar(100)")]
    public string ContentType { get; set; }

    /// <summary>
    /// 文件大小，单位字节
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 是否公开访问
    /// </summary>
    public bool IsPublic { get; set; }
}