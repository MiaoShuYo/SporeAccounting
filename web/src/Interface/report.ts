/// <summary>
/// 报表类型枚举
/// </summary>
export enum ReportTypeEnum {
    /// <summary>
    /// 月度报表
    /// </summary>
    Monthly = 0,
    /// <summary>
    /// 季度报表
    /// </summary>
    Quarterly = 1,
    /// <summary>
    /// 年度报表
    /// </summary>
    Yearly = 2
}

/// <summary>
/// 报表请求参数
/// </summary>
export interface ReportRequest {
    /// <summary>
    /// 报表类型
    /// </summary>
    reportType: ReportTypeEnum;
    /// <summary>
    /// 年份
    /// </summary>
    year: number;
    /// <summary>
    /// 月份
    /// </summary>
    month?: number;
}

/// <summary>
/// 报表数据项
/// </summary>
export interface ReportDataItem {
    /// <summary>
    /// 年份
    /// </summary>
    year: number;
    /// <summary>
    /// 月份
    /// </summary>
    month: number;
    /// <summary>
    /// 金额
    /// </summary>
    amount: number;
}

/// <summary>
/// 报表响应数据
/// </summary>
export interface ReportResponse {
    /// <summary>
    /// 报表数据列表
    /// </summary>
    data: ReportDataItem[];
} 