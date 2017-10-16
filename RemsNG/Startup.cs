using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Security;
using RemsNG.Services;
using RemsNG.Utilities;
using System.IO;
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RemsDbContext dbContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile("Logs/remsng-logs-{Date}.txt");
            DbInitializer.Initialize(dbContext);
            app.UseCors("CorsPolicy");

            app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                    if (result?.Principal != null)
                    {
                        context.User = result.Principal;
                    }
                }

                await next.Invoke();
            });

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value)
                && !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/";
                    await next();
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
