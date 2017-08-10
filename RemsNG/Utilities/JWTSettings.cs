using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class JWTSettings
    {
        private static IConfigurationRoot _Configuration;

        public static IConfigurationRoot Configuration
        {
            get { return _Configuration; }
            set { _Configuration = value; }
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

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.secret)])),

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero,
                AuthenticationType = JwtBearerDefaults.AuthenticationScheme
            };
        }

        public static void Initialize(IApplicationBuilder app, IConfigurationRoot config)
        {
            Configuration = config;
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = false,
                TokenValidationParameters = tokenValidationParameters(),
                Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Response response = new Response();

                        if (context.Exception.GetType() == typeof(SecurityTokenValidationException))
                        {
                            response.description = "invalid token";
                            response.code = MSgCode_Enum.INVALID_TOKEN;
                            //  throw new Exception(MsgCodes.INVALID_TOKEN);
                        }
                        else if (context.Exception.GetType() == typeof(SecurityTokenInvalidIssuerException))
                        {
                            response.description = "INVALID ISSUER";
                            response.code = MSgCode_Enum.INVALID_ISSUER;
                            //   throw new Exception(MsgCodes.INVALID_ISSUER);
                        }
                        else if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            response.description = "TOKEN EXPIRED";
                            response.code = MSgCode_Enum.TOKEN_EXPIRED;
                            // throw new Exception(MsgCodes.TOKEN_EXPIRED);
                        }

                        context.Response.StatusCode = 401;
                        string val = JsonConvert.SerializeObject(response);
                        context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(val), 0, val.Length);
                        context.HandleResponse();

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
