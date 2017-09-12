using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                //options.Audience = "http://localhost:5001/";
                //options.Authority = "http://localhost:5000/";
                options.TokenValidationParameters = tokenValidationParameters();
                options.Events = GetJwtEvt();
            });

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.secret)])), SecurityAlgorithms.HmacSha256);
                options.logOutTIme = jwtAppSettingOptions[nameof(JwtIssuerOptions.logOutTIme)];
            });

            services.AddDbContext<RemsDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IUserService, UserService>();

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
        public static JwtBearerEvents GetJwtEvt()
        {
            return new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Response response = new Response();

                    if (context.Exception.GetType() == typeof(SecurityTokenValidationException))
                    {
                        response.description = "invalid token";
                        response.code = MsgCode_Enum.INVALID_TOKEN;
                        //  throw new Exception(MsgCodes.INVALID_TOKEN);
                    }
                    else if (context.Exception.GetType() == typeof(SecurityTokenInvalidIssuerException))
                    {
                        response.description = "INVALID ISSUER";
                        response.code = MsgCode_Enum.INVALID_ISSUER;
                        //   throw new Exception(MsgCodes.INVALID_ISSUER);
                    }
                    else if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        response.description = "TOKEN EXPIRED";
                        response.code = MsgCode_Enum.TOKEN_EXPIRED;
                        // throw new Exception(MsgCodes.TOKEN_EXPIRED);
                    }

                    context.Response.StatusCode = 401;
                    string val = JsonConvert.SerializeObject(response);
                    //context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(val), 0, val.Length);
                    // context.HandleResponse();

                    return Task.FromResult<object>(val);
                }
            };
        }
    }
}
