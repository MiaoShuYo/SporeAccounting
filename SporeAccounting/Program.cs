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
using SporeAccounting.Server;
using SporeAccounting.Server.Interface;
using SporeAccounting.Middlewares;
using SporeAccounting.Task.Timer;

namespace SporeAccounting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            ConfigurationManager configurationManager = builder.Configuration;
            // ���� JWT ��֤
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
                //��Ӱ�ȫ����
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "������token,��ʽΪ Bearer XXXXX��ע���м�����пո�",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                //��Ӱ�ȫҪ��
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
                    //��ȡ��֤ʧ�ܵ�ģ���ֶ� 
                    var errors = actionContext.ModelState
                        .Where(s => s.Value != null && s.Value.ValidationState == ModelValidationState.Invalid)
                        .SelectMany(s => s.Value!.Errors.ToList())
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    // ͳһ���ظ�ʽ
                    var result = new ResponseData<string>(HttpStatusCode.BadRequest,
                        string.Join("\r\n", errors.ToArray()), "");
                    return new BadRequestObjectResult(result);
                };
            });
            builder.Services.AddDbContext<SporeAccountingDBContext>(ServiceLifetime.Scoped);
            builder.Services.AddScoped(typeof(ISysUserServer), typeof(SysUserImp));
            builder.Services.AddScoped(typeof(ISysRoleServer), typeof(SysRoleImp));
            builder.Services.AddScoped(typeof(ISysRoleUrlServer), typeof(SysRoleUrlImp));
            builder.Services.AddScoped(typeof(ISysUrlServer), typeof(SysUrlImp));
            builder.Services.AddScoped(typeof(IIncomeExpenditureClassificationServer),
                typeof(IncomeExpenditureClassificationImp));
            builder.Services.AddScoped(typeof(ICurrencyServer), typeof(CurrencyImp));
            builder.Services.AddScoped(typeof(IExchangeRateRecordServer), typeof(ExchangeRateRecordImp));
            builder.Services.AddHttpClient();
            // ��Ӷ�ʱ����
            builder.Services.AddQuartz(q =>
            {
                var exchangeRateTimerJobKey=new JobKey("ExchangeRateTimer");
                q.AddJob<ExchangeRateTimer>(opts=>opts.WithIdentity(exchangeRateTimerJobKey));
                q.AddTrigger(opts=>opts
                    .ForJob(exchangeRateTimerJobKey)
                    .WithIdentity("ExchangeRateTimerTrigger")
                    .StartNow()
                    .WithCronSchedule("0 0 1 * * ?"));
            });
            builder.Services.AddQuartzHostedService(options =>
            {
                //���� Quartz ���йܷ���`WaitForJobsToComplete = true` ��ʾ��Ӧ�ó���ֹͣʱ�ȴ�������ɺ��ٹرա�
                options.WaitForJobsToComplete = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<PermissionsMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}