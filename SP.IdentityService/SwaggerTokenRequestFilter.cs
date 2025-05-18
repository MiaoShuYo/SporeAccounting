using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SP.IdentityService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SP.IdentityService;

public class SwaggerTokenRequestFilter: IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath == "connect/token" &&
            context.ApiDescription.HttpMethod == "POST")
        {
            // 配置支持表单提交
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["grant_type"] = new OpenApiSchema { Type = "string", Description = "授权类型", Example = new OpenApiString("password") },
                                ["username"] = new OpenApiSchema { Type = "string", Description = "用户名", Example = new OpenApiString("admin") },
                                ["password"] = new OpenApiSchema { Type = "string", Description = "密码", Example = new OpenApiString("123*asdasd") },
                                ["scope"] = new OpenApiSchema { Type = "string", Description = "授权范围", Example = new OpenApiString("api") },
                                ["client_id"] = new OpenApiSchema { Type = "string", Description = "客户端ID", Example = new OpenApiString("") }
                            },
                            Required = new HashSet<string> { "grant_type" }
                        }
                    }
                }
            };
        }
    }
}