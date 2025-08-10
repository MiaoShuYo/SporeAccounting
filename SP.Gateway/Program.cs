using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Nacos;
using Nacos.V2.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Nacos 配置中心
builder.Services.AddNacosV2Naming(builder.Configuration);
builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));

// 基础服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger 配置
builder.Services.AddSwaggerGen();

// 添加 HTTP 客户端用于获取微服务的 OpenAPI 文档
builder.Services.AddHttpClient();

// Ocelot + Nacos 服务发现
builder.Services.AddOcelot(builder.Configuration)
    .AddNacosDiscovery();

var app = builder.Build();

// 中间件管道
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 启用 Ocelot 作为网关
await app.UseOcelot();

app.Run();