using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RemsNG.ORM;
using RemsNG.Services;
using RemsNG.Utilities;
using System.IO;

namespace RemsNG
{
    public class Startup
    {
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            ServicesCollection.Initialize(services,Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, RemsDbContext dbContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile("Logs/remsng-logs-{Date}.txt");
            DbInitializer.Initialize(dbContext);
            //app.UseCors(builder => builder.WithOrigins("http://localhost:4200")
            //.AllowAnyHeader());
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

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
