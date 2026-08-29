using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.ResourceService.Models.Entity;

/// <summary>
/// 图片文字
/// </summary>
[Table(name: "image_text")]
public class ImageText : BaseModel
{
    /// <summary>
    /// 业务id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long FileId { get; set; }

    /// <summary>
    /// 识别到的文字
    /// </summary>
    [Column(TypeName = "text")]
    public string RecognizedText { get; set; } = "";
}