using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Security;
using RemsNG.Services;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG
{
    public class Startup
    {
        //        private readonly ILogger _logger;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        private ILoggerFactory loggerFactory;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (loggerFactory == null)
            {
                loggerFactory = (ILoggerFactory)new LoggerFactory();
            }
            ServicesCollection.Initialize(services, Configuration, loggerFactory);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RemsDbContext dbContext, IUserService userService)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile("Logs/remsng-logs-{Date}.txt");
            DbInitializer.Initialize(dbContext);
            app.UseCors("CorsPolicy");

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.Use(async (context, next) =>
            {
                if ((context.Response.StatusCode == 404 || !Path.HasExtension(context.Request.Path.Value))
                && !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/";
                }
                else if (!context.User.Identity.IsAuthenticated)
                {
                    string token = context.Request.Headers["Authorization"];
                    if (token != null)
                    {
                        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                        token = token.Replace("Bearer ", "");
                        try
                        {
                            JwtSecurityToken jst = handler.ReadJwtToken(token);
                            DateTime validTo = jst.ValidTo;
                            int compareDate = validTo.CompareTo(DateTime.UtcNow);
                            double times = DateTime.UtcNow.Subtract(validTo).TotalMinutes;
                            if (compareDate < 0 && times <= 10)
                            {
                                // change token
                                var claims = new List<Claim>(jst.Claims);
                                string newToken = userService.GetToken(claims.ToArray());
                                if (!string.IsNullOrEmpty(newToken))
                                {
                                    token = newToken;
                                    //context
                                    IHeaderDictionary headers = context.Response.Headers;
                                    headers["Access-Control-Expose-Headers"] = $"\'new-t\'";
                                    headers["new-t"] = newToken;
                                }
                            }

                            SecurityToken validatedToken;
                            context.User = handler.ValidateToken(token, ServicesCollection.tokenValidationParameters(), out validatedToken);
                            await next.Invoke();
                        }
                        catch (Exception ex)
                        {
                            await ErrorHandlingMiddleware.HandleExceptionAsync(context, ex);
                        }
                    }
                    else
                    {
                        await next.Invoke();
                    }
                }
            });

            app.UseExceptionHandler(builder =>
            {
                builder.Run(
                    async context =>
                    {
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        Response response = new Response();
                        ExceptionTranslator ex = new ExceptionTranslator(loggerFactory);
                        ex.Translate(context, error.Error, response);
                        var result = JsonConvert.SerializeObject(response);
                        await context.Response.WriteAsync(result);
                    });
            });


            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
