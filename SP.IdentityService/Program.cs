using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using SP.Common.Redis;
using SP.IdentityService.DB;
using SP.IdentityService.Models.Entity;
using SP.Common.ConfigService;
using SP.Common.Message.Mq;
using SP.Common.Message.Email;
using SP.IdentityService.Impl;
using SP.IdentityService.Service;
using SP.IdentityService.Service.Impl;
using SP.Common;
using SP.Common.Logger;
using SP.Common.Message.Mq.Consumer;
using SP.Common.Message.SmS;
using SP.Common.Middleware;
using SP.IdentityService;
using SP.IdentityService.Middleware;
using SP.IdentityService.Mq;
using SP.IdentityService.Services;
using SP.IdentityService.Service.Impl;
using SP.Common.ExceptionHandling;
using SP.Common.Nacos;
using SP.Common.Nacos.Configuration;


var builder = WebApplication.CreateBuilder(args);
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
// Add services to the container.
builder.Services.AddControllers();
builder.Services.ConfigureDetailedModelValidation();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Nacos 配置中心（OpenAPI）
builder.Configuration.AddSpNacosConfiguration(builder.Configuration.GetSection("nacos"));

// 注册 SP.Common Nacos OpenAPI 封装
builder.Services.AddSpNacos(builder.Configuration);
// 引入redis
builder.Services.AddRedisService(builder.Configuration);

// 注册DbContext，使用MySQL
builder.Services.AddDbContext<IdentityServerDbContext>(ServiceLifetime.Scoped);
// 注册短信服务
builder.Services.AddTwilioSmSService(builder.Configuration);

// 添加ASP.NET Core Identity服务
builder.Services.AddIdentity<SpUser, SpRole>(options =>
    {
        // 密码策略配置
        options.Password.RequireDigit = true; // 要求数字
        options.Password.RequireLowercase = true; // 要求小写字母
        options.Password.RequireUppercase = true; // 要求大写字母
        options.Password.RequireNonAlphanumeric = true; // 要求特殊字符
        options.Password.RequiredLength = 6; // 最小长度
        options.Password.RequiredUniqueChars = 4; // 要求不同字符的最小数量

        // 锁定设置
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // 锁定15分钟
        options.Lockout.MaxFailedAccessAttempts = 5; // 5次失败尝试后锁定
        options.Lockout.AllowedForNewUsers = true; // 对新用户启用锁定

        // 禁用用户名和邮箱规范化
        options.User.RequireUniqueEmail = false;
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    })
    .AddEntityFrameworkStores<IdentityServerDbContext>()
    .AddDefaultTokenProviders();

// 替换默认的 UserStore
builder.Services.AddScoped<IUserStore<SpUser>, SPUserStore>();
// 添加OpenIddict
builder.Services.AddOpenIddict(builder.Configuration, builder.Environment);

// 注册短信mq
builder.Services.AddHostedService<SmSConsumerService>();
// 注册邮箱mq
builder.Services.AddHostedService<EmailConsumerService>();
builder.Services.AddHostedService<DeadLetterConsumerService>();

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    // 添加XML文档
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // 添加SwaggerTokenRequestFilter
    c.OperationFilter<SwaggerTokenRequestFilter>();

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
            new string[] { }
        }
    });
});

// 注册 ContextSession
builder.Services.AddScoped<ContextSession>();

builder.Services.AddScoped<IAuthorizationService, AuthorizationServiceImpl>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IRoleService, RoleServiceImpl>();

// 注册客户端注册服务
builder.Services.AddScoped<IClientRegistrationService, ClientRegistrationService>();
builder.Services.AddScoped<ICaptchaService, CaptchaServiceImpl>();

builder.Services.AddSingleton<EmailConfigService>();
builder.Services.AddSingleton<RabbitMqConfigService>();
builder.Services.AddSingleton(provider =>
{
    var configService = provider.GetRequiredService<EmailConfigService>();
    return new EmailMessage(configService.GetEmailConfig());
});
builder.Services.AddSingleton<RabbitMqMessage>(provider =>
{
    var configService = provider.GetRequiredService<RabbitMqConfigService>();
    var logger = provider.GetRequiredService<ILogger<RabbitMqMessage>>();
    return new RabbitMqMessage(logger, configService.GetRabbitMqConfig());
});
// 注册JwtConfigService（ApplicationMiddleware需要）
builder.Services.AddSingleton<JwtConfigService>();
// 注入loki日志服务
builder.Services.AddLoggerService(builder.Configuration);

// 配置转发头中间件，用于获取反向代理后的真实客户端 IP
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto;
    // 生产环境建议配置 KnownProxies/KnownNetworks 降低信任范围
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// 启动时迁移数据库并进行幂等初始化（角色与管理员用户）
// 仅在 Development/Local 环境上执行默认用户名/密码初始化
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<IdentityServerDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<SpRole>>();
        string[] roleNames = new[] { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                var createResult = await roleManager.CreateAsync(new SpRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });
                if (!createResult.Succeeded)
                {
                    logger.LogWarning("创建角色 {Role} 失败：{Errors}", roleName,
                        string.Join(",", createResult.Errors.Select(e => e.Description)));
                }
            }
        }

        // 管理员初始化仅在 Development/Local 环境执行，且密码从配置读取
        if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
        {
            var userManager = services.GetRequiredService<UserManager<SpUser>>();
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                // 管理员账号和密码从配置读取，禁止硬编码
                var adminEmail = builder.Configuration["AdminInit:Email"]
                    ?? throw new InvalidOperationException("缺少管理员考配置：AdminInit:Email");
                var adminPassword = builder.Configuration["AdminInit:Password"]
                    ?? throw new InvalidOperationException("缺少管理员考配置：AdminInit:Password");

                logger.LogWarning("开发环境：正在初始化 admin 用户，请确保生产环境不运行此初始化逻辑");
                var newAdminUser = new SpUser { UserName = "admin", Email = adminEmail, EmailConfirmed = true };
                var createAdmin = await userManager.CreateAsync(newAdminUser, adminPassword);
                if (!createAdmin.Succeeded)
                {
                    logger.LogWarning("创建管理员用户失败：{Errors}",
                        string.Join(",", createAdmin.Errors.Select(e => e.Description)));
                    return;
                }
                adminUser = await userManager.FindByNameAsync("admin");
            }

            if (adminUser == null)
            {
                logger.LogWarning("管理员用户初始化失败：未能读取 admin 用户");
                return;
            }

            // 确保管理员在 Admin 角色中
            var inRole = await userManager.IsInRoleAsync(adminUser, "Admin");
            if (!inRole)
            {
                var addToRole = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!addToRole.Succeeded)
                {
                    logger.LogWarning("将管理员加入 Admin 角色失败：{Errors}",
                        string.Join(",", addToRole.Errors.Select(e => e.Description)));
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "启动初始化失败");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseRouting();

// 添加认证中间件
app.UseAuthentication();
app.UseMiddleware<ApplicationMiddleware>();
app.UseAuthorization();

// 添加 Token 存储中间件
app.UseTokenStorage();

app.MapControllers();

app.Run();