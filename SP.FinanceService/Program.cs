using System.Reflection;
using Nacos.AspNetCore.V2;
using Nacos.V2.DependencyInjection;
using SP.Common.ConfigService;
using SP.FinanceService.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加Nacos服务注册
builder.Services.AddNacosAspNet(builder.Configuration);
// 添加Nacos配置中心
builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));
builder.Services.AddNacosV2Naming(builder.Configuration);
// 注册 DbContext
builder.Services.AddDbContext<FinanceServiceDbContext>(ServiceLifetime.Scoped);

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<JwtConfigService>();

var app = builder.Build();

AppDomain.CurrentDomain.SetData("HttpContextAccessor", app.Services.GetService<IHttpContextAccessor>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();