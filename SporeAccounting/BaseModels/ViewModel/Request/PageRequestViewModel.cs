using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.BaseModels.ViewModel.Request;

/// <summary>
/// 分页查询请求基类
/// </summary>
public class PageRequestViewModel
{
    /// <summary>
    /// 请求的页码
    /// </summary>
    [Range(1, int.MaxValue,ErrorMessage = $"{nameof(PageNumber)}不能小于1大于2147483647")]
    [Required(ErrorMessage = $"{nameof(PageNumber)}不能为空")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    [Range(1, 50, ErrorMessage = $"{nameof(PageSize)}不能小于1大于50")]
    [Required(ErrorMessage = $"{nameof(PageSize)}不能为空")]
    public int PageSize { get; set; } = 20;

}