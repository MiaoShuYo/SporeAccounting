using System.Reflection;
using Nacos.AspNetCore.V2;
using Nacos.V2.DependencyInjection;
using SP.Common;
using SP.Common.Middleware;
using SP.ReportService.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 添加XML文档
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 添加Nacos服务注册
builder.Services.AddNacosAspNet(builder.Configuration);
// 添加Nacos配置中心
builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));
builder.Services.AddNacosV2Naming(builder.Configuration);
// 注册 DbContext
builder.Services.AddDbContext<ReportServiceDBContext>(ServiceLifetime.Scoped);
// 注册 IHttpContextAccessor
builder.Services.AddHttpContextAccessor(); 
// 注册 ContextSession
builder.Services.AddScoped<ContextSession>(); 
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ApplicationMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();