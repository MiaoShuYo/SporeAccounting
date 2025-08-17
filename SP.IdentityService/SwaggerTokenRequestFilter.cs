using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SP.IdentityService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SP.IdentityService;

public class SwaggerTokenRequestFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath == "api/auth/token" &&
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
                            Description = "表单提交参数，必须使用表单提交，不要将参数放在URL中",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["grant_type"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "授权类型，支持 password、refresh_token、client_credentials",
                                    Example = new OpenApiString("password")
                                },
                                ["username"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "用户名（password模式下需要）",
                                    Example = new OpenApiString("admin")
                                },
                                ["password"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "密码（password模式下需要）",
                                    Example = new OpenApiString("123*asdasd")
                                },
                                ["refresh_token"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "刷新令牌（refresh_token模式下需要）"
                                },
                                ["scope"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "授权范围，可选值: api 或 api offline_access",
                                    Example = new OpenApiString("api offline_access")
                                },
                                ["client_id"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "客户端ID（密码模式下可选）",
                                    Example = new OpenApiString("")
                                }
                            },
                            Required = new HashSet<string> { "grant_type" }
                        }
                    }
                }
            };

            // 配置响应示例
            operation.Responses["200"] = new OpenApiResponse
            {
                Description = "认证成功，返回访问令牌",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["access_token"] = new OpenApiSchema { Type = "string", Description = "访问令牌" },
                                ["token_type"] = new OpenApiSchema
                                    { Type = "string", Description = "令牌类型", Example = new OpenApiString("Bearer") },
                                ["expires_in"] = new OpenApiSchema
                                    { Type = "integer", Description = "有效期（秒）", Example = new OpenApiInteger(1800) },
                                ["refresh_token"] = new OpenApiSchema { Type = "string", Description = "刷新令牌" },
                                [".issued"] = new OpenApiSchema { Type = "string", Description = "颁发时间" },
                                [".expires"] = new OpenApiSchema { Type = "string", Description = "过期时间" }
                            }
                        }
                    }
                }
            };
        }
    }
}