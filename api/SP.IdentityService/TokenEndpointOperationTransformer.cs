using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SP.IdentityService;

public class TokenEndpointOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.Description.RelativePath == "api/auth/token" &&
            string.Equals(context.Description.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
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
                            Type = JsonSchemaType.Object,
                            Description = "表单提交参数，必须使用表单提交，不要将参数放在URL中",
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["grant_type"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "授权类型，支持 password、refresh_token、client_credentials，自定义：sms_otp、email_code",
                                    Example = JsonValue.Create("password"),
                                    Enum = new List<JsonNode>
                                    {
                                        JsonValue.Create("password")!,
                                        JsonValue.Create("refresh_token")!,
                                        JsonValue.Create("client_credentials")!,
                                        JsonValue.Create("sms_otp")!,
                                        JsonValue.Create("email_code")!
                                    }
                                },
                                ["username"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "用户名（password 模式下需要）",
                                    Example = JsonValue.Create("admin")
                                },
                                ["password"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "密码（password 模式下需要）",
                                    Example = JsonValue.Create("123*asdasd")
                                },
                                ["refresh_token"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "刷新令牌（refresh_token 模式下需要）"
                                },
                                ["scope"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "授权范围，可选值: api 或 api offline_access",
                                    Example = JsonValue.Create("api offline_access")
                                },
                                ["client_id"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "客户端ID（password 模式下可选）",
                                    Example = JsonValue.Create("")
                                },
                                // 自定义授权类型：短信验证码登录
                                ["phone_number"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "手机号（sms_otp 模式下需要）",
                                    Example = JsonValue.Create("13800000000")
                                },
                                ["code"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "验证码（sms_otp / email_code 模式下需要）",
                                    Example = JsonValue.Create("123456")
                                },
                                // 自定义授权类型：邮箱验证码登录
                                ["email"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "邮箱（email_code 模式下需要）",
                                    Example = JsonValue.Create("user@example.com")
                                },
                                // 可选：图形验证码
                                ["captcha_token"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "图形验证码token（如启用防刷）"
                                },
                                ["captcha_code"] = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Description = "图形验证码文本（如启用防刷）"
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
                            Type = JsonSchemaType.Object,
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["access_token"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "访问令牌" },
                                ["token_type"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "令牌类型", Example = JsonValue.Create("Bearer") },
                                ["expires_in"] = new OpenApiSchema { Type = JsonSchemaType.Integer, Description = "有效期（秒）", Example = JsonValue.Create(1800) },
                                ["refresh_token"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "刷新令牌" },
                                [".issued"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "颁发时间" },
                                [".expires"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "过期时间" }
                            }
                        }
                    }
                }
            };
        }

        return Task.CompletedTask;
    }
}
