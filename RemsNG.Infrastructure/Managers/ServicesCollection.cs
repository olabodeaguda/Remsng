﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RemsNG.Common.Models;
using RemsNG.Models;
using RemsNG.Security;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ServicesCollection
    {
        private static IConfigurationRoot _Configuration;

        public static IConfigurationRoot Configuration
        {
            get => _Configuration;
            set => _Configuration = value;
        }

        public static void Initialize(IServiceCollection services, IConfigurationRoot config,
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

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IDomainService, DomainService>();
            services.AddTransient<ILcdaService, LcdaService>();
            services.AddTransient<IWardService, WardService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IPermission, PermissionManagers>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IStreetService, StreetManagers>();
            services.AddTransient<ISectorService, SectorManagers>();
            services.AddTransient<IItemService, ItemManagers>();
            services.AddTransient<IItemPenaltyService, ItemPenaltyService>();
            services.AddTransient<ITaxpayerCategoryService, TaxpayerCategoryService>();
            services.AddTransient<ITaxpayerService, TaxpayerService>();
            services.AddTransient<ICompany, CompanyManagers>();
            services.AddTransient<IAddress, AddressService>();
            services.AddTransient<ICompanyItemService, CompanyItemManagers>();
            services.AddTransient<IDemandNoticeService, DemanNoticeService>();
            services.AddTransient<IRunDemandNoticeService, RunDemandNoticeService>();
            services.AddTransient<IDnTaxpayer, DnTaxpayerService>();
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IDemandNoticeTaxpayerService, DemandNoticeTaxpayerManagers>();
            services.AddTransient<IDemandNoticeItemService, DemandNoticeItemManagers>();
            services.AddTransient<IDnDownloadService, DnDownloadService>();
            services.AddTransient<IDemandNoticeCharges, DemandNoticeChargesManagers>();
            services.AddTransient<IDemandNoticeCharges, DemandNoticeChargesManagers>();
            services.AddTransient<IDemandNoticeDownloadHistory, DemandNoticeDownloadHistoryManagers>();
            services.AddTransient<ILcdaBankService, LcdaBankService>();
            services.AddTransient<IBatchDwnRequestService, BatchDwnRequestService>();
            services.AddTransient<IListPropertyService, ListPropertyService>();
            services.AddTransient<IDNAmountDueMgtService, DNAmountDueMgtService>();
            services.AddTransient<IDNPaymentHistoryService, DNPaymentHistoryService>();
            services.AddTransient<IBankService, BankManagers>();
            services.AddTransient<IAbstractService, AbstractManagers>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddTransient<ISyncService, SyncManagers>();
            services.AddSingleton<IConfigurationRoot>(provider => Configuration);
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