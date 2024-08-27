using System.Net;

namespace SporeAccounting.BaseModels;

/// <summary>
/// 返回给客户端的响应封装
/// </summary>
public class ResponseData<T>
{
    /// <summary>
    /// 返回给客户端的响应封装
    /// </summary>
    /// <param name="statusCode">http 状态码</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="data">返回数据</param>
    public ResponseData(HttpStatusCode statusCode, string errorMessage="", T data=default(T))
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
        Data = data;
    }

    /// <summary>
    /// 响应的Code
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; }
    /// <summary>
    /// 数据
    /// </summary>
    public T Data { get; set; }
}