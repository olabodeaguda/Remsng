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
using RemsNG.Infrastructure.Services;
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
                        var result = JsonConvert.SerializeObject(response,
                            new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

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
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IDomainManager, DomainManager>();
            services.AddTransient<ILcdaManager, LcdaManager>();
            services.AddTransient<IWardManager, WardManager>();
            services.AddTransient<IRoleManager, RoleManager>();
            services.AddTransient<IPermissionManager, PermissionManager>();
            services.AddTransient<IContactManager, ContactManager>();
            services.AddTransient<IStreetManager, StreetManager>();
            services.AddTransient<ISectorManager, SectorManager>();
            services.AddTransient<IItemManager, ItemManager>();
            services.AddTransient<IItemPenaltyManager, ItemPenaltyManager>();
            services.AddTransient<ITaxpayerCategoryManager, TaxpayerCategoryManager>();
            services.AddTransient<ITaxpayerManager, TaxpayerManager>();
            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<IAddressManager, AddressManager>();
            services.AddTransient<ICompanyItemManager, CompanyItemManager>();
            services.AddTransient<IDemandNoticeManager, DemanNoticeManager>();
            services.AddTransient<IRunDemandNoticeManager, RunDemandNoticeManager>();
            services.AddTransient<IDnTaxpayerManager, DnTaxpayerManagers>();
            services.AddTransient<IStateManagers, StateManagers>();
            services.AddTransient<IImageManager, ImageManager>();
            services.AddTransient<IDemandNoticeTaxpayerManager, DemandNoticeTaxpayerManager>();
            services.AddTransient<IDemandNoticeItemManager, DemandNoticeItemManager>();
            services.AddTransient<IDnDownloadManager, DnDownloadManager>();
            services.AddTransient<IDemandNoticeChargesManager, DemandNoticeChargesManager>();
            services.AddTransient<IDemandNoticeChargesManager, DemandNoticeChargesManager>();
            services.AddTransient<IDemandNoticeDownloadHistoryManager, DemandNoticeDownloadHistoryManager>();
            services.AddTransient<ILcdaBankManager, LcdaBankManager>();
            services.AddTransient<IBatchDwnRequestManager, BatchDwnRequestManager>();
            services.AddTransient<IListPropertyManager, ListPropertyManager>();
            services.AddTransient<IDNAmountDueMgtManager, DNAmountDueMgtManager>();
            services.AddTransient<IDNPaymentHistoryManager, DNPaymentHistoryManager>();
            services.AddTransient<IBankManager, BankManager>();
            services.AddTransient<IAbstractManager, AbstractManager>();
            services.AddTransient<IReportManager, ReportManager>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddTransient<ISyncManager, SyncManager>();
            services.AddTransient<IPenaltyManager, PenaltyManager>();
            services.AddTransient<IArrearsManager, ArrearsManager>(); services.AddSingleton<IConfigurationRoot>(provider => (IConfigurationRoot)Configuration);
            services.AddSingleton(config.GetSection("BankCategory").Get<BankCategory>());
            services.AddSingleton(config.GetSection("TemplateDetail").Get<TemplateDetail>());
            #endregion

            #region Services
            services.AddTransient<IPdfService, PdfService>();
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
