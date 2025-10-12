using System.Reflection;
using SP.MLService.Services;
using SP.MLService.Domain;
using Nacos.V2.DependencyInjection;
using Nacos.AspNetCore.V2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 添加XML文档
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // 添加JWT认证配置
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 覆盖 Nacos 注册的 IP/Port 为宿主机IP + 对外端口（通过环境变量或配置传入）
var hostIp = Environment.GetEnvironmentVariable("HOST_IP") ?? builder.Configuration["HOST_IP"];
var exposePort = Environment.GetEnvironmentVariable("EXPOSE_PORT") ?? builder.Configuration["EXPOSE_PORT"];

if (!string.IsNullOrWhiteSpace(hostIp) || !string.IsNullOrWhiteSpace(exposePort))
{
    var overrides = new Dictionary<string, string?>();
    if (!string.IsNullOrWhiteSpace(hostIp)) overrides["nacos:Ip"] = hostIp;
    if (!string.IsNullOrWhiteSpace(exposePort)) overrides["nacos:Port"] = exposePort;
    builder.Configuration.AddInMemoryCollection(overrides);
}
// 添加Nacos服务注册
builder.Services.AddNacosAspNet(builder.Configuration);
// 添加Nacos配置中心
builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));
builder.Services.AddNacosV2Naming(builder.Configuration);

// 配置 CORS（允许前端调用）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 注册渐进式学习管理器（MongoDB版本）
builder.Services.AddSingleton<ProgressiveLearningManager>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var modelPath = config["ML:ModelPath"] ?? Path.Combine(AppContext.BaseDirectory, "Models", "category_model.zip");
    
    // MongoDB配置
    var connectionString = config["ML:MongoDB:ConnectionString"] ?? "mongodb://admin:admin@14.103.224.141:27017/admin";
    var databaseName = config["ML:MongoDB:DatabaseName"] ?? "SporeAccountingML";
    var collectionName = config["ML:MongoDB:FeedbackCollectionName"] ?? "UserFeedbacks";
    
    var options = new ProgressiveLearningOptions
    {
        ConfidenceThreshold = config.GetValue<float>("ML:ConfidenceThreshold", 0.4f),
        MinTrainingDataSize = config.GetValue<int>("ML:MinTrainingDataSize", 10),
        RetrainingFrequency = config.GetValue<int>("ML:RetrainingFrequency", 5),
        IncrementalBatchSize = config.GetValue<int>("ML:IncrementalBatchSize", 10)
    };
    
    return new ProgressiveLearningManager(modelPath, connectionString, databaseName, collectionName, options);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// 启动时显示信息
var learningManager = app.Services.GetRequiredService<ProgressiveLearningManager>();
var stats = learningManager.GetStats();
Console.WriteLine($"=== SporeAccountingML API 已启动 ===");
Console.WriteLine($"反馈数: {stats.TotalFeedbacks}, 训练数据: {stats.TrainingDataSize}, 模型存在: {stats.ModelExists}");

app.Run();

// 单例 MLContext 工具
public static class MLContextSingleton
{
    private static readonly Microsoft.ML.MLContext _ml = new(1);
    public static Microsoft.ML.IDataView GetSchemaFromEnumerable(IEnumerable<LtrRow> rows)
    {
        return _ml.Data.LoadFromEnumerable(rows);
    }
}
