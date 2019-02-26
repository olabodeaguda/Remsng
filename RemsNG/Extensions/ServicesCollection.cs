using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Services;
using RemsNG.Common.Models;
using RemsNG.Infrastructure.Managers;
using RemsNG.Models;
using RemsNG.Security;
using RemsNG.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Extensions
{
    public class ServicesCollection
    {
        private static IConfiguration _Configuration;

        public static IConfiguration Configuration
        {
            get => _Configuration;
            set => _Configuration = value;
        }

        public static void Initialize(IServiceCollection services, IConfiguration config,
            ILoggerFactory loggerFactory)
        {
            Configuration = config;

            var builderException = services.AddMvc();

            services.AddNodeServices(x =>
            {
                x.InvocationTimeoutMilliseconds = 6000000;
                x.LaunchWithDebugging = false;
            });

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.secret)])), SecurityAlgorithms.HmacSha256);
                options.logOutTIme = jwtAppSettingOptions[nameof(JwtIssuerOptions.logOutTIme)];
            });

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(
            options =>
            {
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.TokenValidationParameters = tokenValidationParameters();
                options.IncludeErrorDetails = true;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Response response = new Response();
                        Exception except = context.Exception;

                        ExceptionTranslator ex = new ExceptionTranslator(loggerFactory);
                        ex.Translate(context.HttpContext, context.Exception, response);
                        var result = JsonConvert.SerializeObject(response);

                        context.Fail(result);

                        return Task.FromException(except);// CompletedTask;
                    }
                };
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build());
            });
            var corsUrls = new List<string>() { "http://localhost:4200" };

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithExposedHeaders("new-t")
                  .WithOrigins(corsUrls.ToArray());
            }));

            services.Configure<MvcOptions>(
                options =>
                {
                    options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
                });

            #region service DI

            services.AddTransient<IUserManagers, UserManagers>();
            services.AddTransient<IDomainManagers, DomainManagers>();
            services.AddTransient<ILcdaManagers, LcdaManagers>();
            services.AddTransient<IWardManagers, WardManagers>();
            services.AddTransient<IRoleManagers, RoleManagers>();
            services.AddTransient<IPermissionManagers, PermissionManagers>();
            services.AddTransient<IContactManagers, ContactManagers>();
            services.AddTransient<IStreetManagers, StreetManagers>();
            services.AddTransient<ISectorManagers, SectorManagers>();
            services.AddTransient<IItemManagers, ItemManagers>();
            services.AddTransient<IItemPenaltyManagers, ItemPenaltyManagers>();
            services.AddTransient<ITaxpayerCategoryManagers, TaxpayerCategoryManagers>();
            services.AddTransient<ITaxpayerManagers, TaxpayerManagers>();
            services.AddTransient<ICompanyManagers, CompanyManagers>();
            services.AddTransient<IAddressManagers, AddressManagers>();
            services.AddTransient<ICompanyItemManagers, CompanyItemManagers>();
            services.AddTransient<IDemandNoticeManagers, DemanNoticeManager>();
            services.AddTransient<IRunDemandNoticeManagers, RunDemandNoticeManagers>();
            services.AddTransient<IDnTaxpayerManagers, DnTaxpayerManagers>();
            services.AddTransient<IStateManagers, StateManagers>();
            services.AddTransient<IImageManagers, ImageManagers>();
            services.AddTransient<IDemandNoticeTaxpayerManagers, DemandNoticeTaxpayerManagers>();
            services.AddTransient<IDemandNoticeItemManagers, DemandNoticeItemManagers>();
            services.AddTransient<IDnDownloadManagers, DnDownloadManagers>();
            services.AddTransient<IDemandNoticeChargesManagers, DemandNoticeChargesManagers>();
            services.AddTransient<IDemandNoticeChargesManagers, DemandNoticeChargesManagers>();
            services.AddTransient<IDemandNoticeDownloadHistoryManagers, DemandNoticeDownloadHistoryManagers>();
            services.AddTransient<ILcdaBankManagers, LcdaBankManagers>();
            services.AddTransient<IBatchDwnRequestManagers, BatchDwnRequestManagers>();
            services.AddTransient<IListPropertyManagers, ListPropertyManagers>();
            services.AddTransient<IDNAmountDueMgtManagers, DNAmountDueMgtManagers>();
            services.AddTransient<IDNPaymentHistoryManagers, DNPaymentHistoryManagers>();
            services.AddTransient<IBankManagers, BankManagers>();
            services.AddTransient<IAbstractManagers, AbstractManagers>();
            services.AddTransient<IReportManagers, ReportManagers>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddTransient<ISyncManagers, SyncManagers>();
            services.AddSingleton<IConfigurationRoot>(provider => (IConfigurationRoot)Configuration);
            services.AddSingleton(config.GetSection("BankCategory").Get<BankCategory>());
            
            #endregion
        }

        public static IConfigurationSection jwtAppSettingOptions => Configuration.GetSection(nameof(JwtIssuerOptions));

        public static TokenValidationParameters tokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                ValidateAudience = false,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.secret)])),
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                AuthenticationType = JwtBearerDefaults.AuthenticationScheme
            };
        }
    }
}
