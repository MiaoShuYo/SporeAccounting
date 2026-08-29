using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace SP.Common.ExceptionHandling
{
    public static class ModelValidationExtensions
    {
        /// <summary>
        /// 统一配置 ModelState 无效时的返回，仅返回错误消息数组
        /// </summary>
        public static IServiceCollection ConfigureDetailedModelValidation(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var messages = context.ModelState
                        .Where(kvp => kvp.Value != null && kvp.Value.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value!.Errors)
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
                        .Where(m => !string.IsNullOrWhiteSpace(m))
                        .Distinct()
                        .Cast<string>()
                        .ToArray();

                    var response = new ExceptionResponse
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = messages.Length > 0 ? string.Join("；", messages) : "提交的数据无效"
                    };

                    var payload = new
                    {
                        statusCode = response.StatusCode,
                        errorMessage = response.ErrorMessage
                    };

                    return new BadRequestObjectResult(payload)
                    {
                        ContentTypes = { "application/json" }
                    };
                };
            });

            return services;
        }
    }
}


