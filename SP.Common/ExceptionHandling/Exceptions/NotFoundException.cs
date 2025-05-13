using System.Net;

namespace SP.Common.ExceptionHandling
{
    /// <summary>
    /// 资源未找到异常
    /// </summary>
    public class NotFoundException : AppException
    {
        /// <summary>
        /// 创建一个资源未找到异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        public NotFoundException(string message) 
            : base(message, HttpStatusCode.NotFound)
        {
        }

        /// <summary>
        /// 创建一个资源未找到异常实例
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">内部异常</param>
        public NotFoundException(string message, Exception innerException) 
            : base(message, innerException, HttpStatusCode.NotFound)
        {
        }

        /// <summary>
        /// 创建一个资源未找到异常实例，使用默认消息格式
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="id">资源ID</param>
        public NotFoundException(string resourceName, object id)
            : base($"未找到{resourceName}: {id}", HttpStatusCode.NotFound)
        {
        }
    }
} 