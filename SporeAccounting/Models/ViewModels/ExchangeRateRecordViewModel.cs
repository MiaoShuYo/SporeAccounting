namespace SporeAccounting.Models.ViewModels;

public class ExchangeRateRecordViewModel
{
    /// <summary>
    /// 币种转换
    /// </summary>
    public string ConvertCurrency { get; set; }
    ///<summary>
    /// 币种1
    ///</summary>
    public string Currency1 { get; set; }

    ///<summary>
    /// 币种2
    ///</summary>
    public string Currency2 { get; set; }

    ///<summary>
    /// 汇率
    ///</summary>
    public decimal Rate { get; set; }

    ///<summary>
    /// 更新时间
    ///</summary>
    public DateTime UpdateTime { get; set; }
}