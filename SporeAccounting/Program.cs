using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Quartz;
using SporeAccounting.BaseModels;
using SporeAccounting.Initialization;
using SporeAccounting.Server;
using SporeAccounting.Server.Interface;
using SporeAccounting.Middlewares;
using SporeAccounting.MQ;
using SporeAccounting.MQ.Model;
using SporeAccounting.Task.Timer;

namespace SporeAccounting
{
    /// <summary>
    /// 程序入口
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            ConfigurationManager configurationManager = builder.Configuration;
            // 配置 JWT 验证
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configurationManager["JWT:ValidIssuer"],
                    ValidAudience = configurationManager["JWT:ValidAudience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationManager["JWT:IssuerSigningKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddSwaggerGen(s =>
            {
                var file = Path.Combine(AppContext.BaseDirectory, "SporeAccounting.xml");
                var path = Path.Combine(AppContext.BaseDirectory, file);
                s.IncludeXmlComments(path);
                s.OrderActionsBy(o => o.RelativePath);
                //添加安全定义
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入token,格式为 Bearer XXXXX（注意中间必须有空格）",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //添加安全要求
                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    //获取验证失败的模型字段 
                    var errors = actionContext.ModelState
                        .Where(s => s.Value != null && s.Value.ValidationState == ModelValidationState.Invalid)
                        .SelectMany(s => s.Value!.Errors.ToList())
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    // 统一返回格式
                    var result = new ResponseData<string>(HttpStatusCode.BadRequest,
                        string.Join("\r\n", errors.ToArray()), "");
                    return new BadRequestObjectResult(result);
                };
            });

            #region RabbitMQ配置

            builder.Services.AddSingleton(new RabbitMQOptions
            {
                HostName = configurationManager["RabbitMQ:Host"],
                Port = int.Parse(configurationManager["RabbitMQ:Port"]),
                UserName = configurationManager["RabbitMQ:UserName"],
                Password = configurationManager["RabbitMQ:Password"],
                VirtualHost = configurationManager["RabbitMQ:VirtualHost"],
            });
            builder.Services.AddSingleton<RabbitMQConnection>();
            builder.Services.AddSingleton<RabbitMQPublisher>();

            #endregion

            builder.Services.AddDbContext<SporeAccountingDBContext>(ServiceLifetime.Scoped);
            builder.Services.AddScoped(typeof(ISysUserServer), typeof(SysUserImp));
            builder.Services.AddScoped(typeof(IConfigServer), typeof(ConfigImp));
            builder.Services.AddScoped(typeof(ISysRoleServer), typeof(SysRoleImp));
            builder.Services.AddScoped(typeof(ISysRoleUrlServer), typeof(SysRoleUrlImp));
            builder.Services.AddScoped(typeof(ISysUrlServer), typeof(SysUrlImp));
            builder.Services.AddScoped(typeof(IIncomeExpenditureClassificationServer),
                typeof(IncomeExpenditureClassificationImp));
            builder.Services.AddScoped(typeof(ICurrencyServer), typeof(CurrencyImp));
            builder.Services.AddScoped(typeof(IExchangeRateRecordServer), typeof(ExchangeRateRecordImp));
            builder.Services.AddScoped(typeof(IAccountBookServer), typeof(AccountBookImp));
            builder.Services.AddScoped(typeof(IBudgetServer), typeof(BudgetImp));
            builder.Services.AddScoped(typeof(IIncomeExpenditureRecordServer), typeof(IncomeExpenditureRecordImp));
            builder.Services.AddHttpClient();
            // 注册通用订阅服务
            builder.Services.AddSingleton<RabbitMQSubscriberService>();
            // 注册后台服务，用于启动订阅
            builder.Services.AddHostedService<RabbitMQBackgroundService>();
            // 添加定时任务
            builder.Services.AddQuartz(q =>
            {
                var exchangeRateTimerJobKey = new JobKey("ExchangeRateTimer");
                q.AddJob<ExchangeRateTimer>(opts => opts.WithIdentity(exchangeRateTimerJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(exchangeRateTimerJobKey)
                    .WithIdentity("ExchangeRateTimerTrigger")
                    .StartNow()
                    .WithCronSchedule("0 0 1 * * ?"));
            });
            builder.Services.AddQuartzHostedService(options =>
            {
                //启用 Quartz 的托管服务，`WaitForJobsToComplete = true` 表示在应用程序停止时等待任务完成后再关闭。
                options.WaitForJobsToComplete = true;
            });
            // 配置跨域
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", cp =>
                {
                    cp.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                AddPath.Init(app.Services);
                AddRolePath.Init(app.Services);
            }

            app.UseCors("AllowAll");
            app.UseMiddleware<PermissionsMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}