
using BankingSystem.DB;
using BankingSystem.DB.Entities;
using BankingSystem.Features.ATM.AccountBlance;
using BankingSystem.Features.ATM.ChangePin;
using BankingSystem.Features.ATM.GetBalance;
using BankingSystem.Features.ATM.Withdraw;
using BankingSystem.Features.InternetBank.Auth;
using BankingSystem.Features.InternetBank.Operator.AddAccountForUser;
using BankingSystem.Features.InternetBank.Operator.AddUser;
using BankingSystem.Features.InternetBank.Operator.AddUserDetails;
using BankingSystem.Features.InternetBank.Operator.Auth;
using BankingSystem.Features.InternetBank.Operator.AuthUser;
using BankingSystem.Features.InternetBank.User.GetUserInfo;
using BankingSystem.Features.InternetBank.User.Transactions;
using BankingSystem.Features.Reports;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace BankingSystem
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

            AuthConfigurator.Configure(builder);

            builder.Services.AddTransient<IRegisterUserRepository, RegisterUserRepository>();
            builder.Services.AddTransient<IGetBalanceRepository, GetBalanceRepository>();
            builder.Services.AddTransient<IGetUserInfoRepository, GetUserInfoRepository>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();
            builder.Services.AddTransient<IGetBalanceService, GetBalanceService>();
            builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
            builder.Services.AddTransient<IConvertService, ConvertService>();
            builder.Services.AddTransient<IWithdrawRepository, WithdrawRepository>();
            builder.Services.AddTransient<IWithdrawService, WithdrawService>();
            builder.Services.AddTransient<IChangePinService, ChangePinService>();
            builder.Services.AddTransient<IChangeCardPinRepository, ChangePinRepository>();
            builder.Services.AddTransient<IReportsRepository, ReportsRepository>();
            builder.Services.AddTransient<IReportsService, ReportsService>();
            builder.Services.AddTransient<RegisterUserService>();
            builder.Services.AddTransient<RegisterUserRepository>();
            builder.Services.AddTransient<IAddUserDetailsRepository, AddUserDetailsRepository>();
            builder.Services.AddTransient<AddUserDetailsService>();
            builder.Services.AddTransient<GetUserInfoService>();
            builder.Services.AddTransient<GetUserInfoRepository>();
            builder.Services.AddTransient<AuthService>();

  //          var logger = new LoggerConfiguration()
  //.ReadFrom.Configuration(builder.Configuration)
  //.Enrich.FromLogContext()
  //.CreateLogger();
  //          builder.Logging.ClearProviders();
  //          builder.Logging.AddSerilog(logger);

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWTToken_Auth_API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AppToDb")));
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

