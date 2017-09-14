using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ServicesCollection
    {
        private static IConfigurationRoot _Configuration;

        public static IConfigurationRoot Configuration
        {
            get { return _Configuration; }
            set { _Configuration = value; }
        }

        public static void Initialize(IServiceCollection services, IConfigurationRoot config)
        {
            Configuration = config;
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.secret)])), SecurityAlgorithms.HmacSha256);
                options.logOutTIme = jwtAppSettingOptions[nameof(JwtIssuerOptions.logOutTIme)];
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
            options =>
            {
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.TokenValidationParameters = tokenValidationParameters();
                options.IncludeErrorDetails = true;
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build());
            });

            services.AddDbContext<RemsDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IDomainService, DomainService>();
        }

        public static IConfigurationSection jwtAppSettingOptions
        {
            get
            {
                return Configuration.GetSection(nameof(JwtIssuerOptions));
            }
        }

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
