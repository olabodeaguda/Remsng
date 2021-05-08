using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RemsNG.Common.Models;
using RemsNG.Extensions;
using RemsNG.Filters;
using RemsNG.Security;
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

        public IConfiguration Configuration { get; }
        private ILoggerFactory loggerFactory;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (loggerFactory == null)
            {
                loggerFactory = new LoggerFactory();
            }
            services.AddDatabaseService(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            ServicesCollection.Initialize(services, Configuration, loggerFactory);
            services.AddMvc(config =>
            {
                config.Filters.Add(new GlobalExceptionFilter());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddConsole();
            loggerFactory.AddFile("Logs/remsng-logs-{Date}.txt");
            app.UseCors("CorsPolicy");

            //app.UseHangfireDashboard();

            //var ops = new BackgroundJobServerOptions { WorkerCount = 20 };

            //app.UseHangfireServer(ops);

            app.Use(async (context, next) =>
           {
               if ((context.Response.StatusCode == 404 || !Path.HasExtension(context.Request.Path.Value))
          && !context.Request.Path.Value.StartsWith("/api/"))
               {
                   context.Request.Path = "/";
               }

               await next.Invoke();
           });

            app.UseMiddleware(typeof(TokenValidationMiddleware));

            app.UseExceptionHandler(builder =>
            {
                builder.Run(
                    async context =>
                    {
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        var exception = error.Error;


                        var (responseModel, statusCode) = GlobalExceptionFilter.GetStatusCode<object>(exception);
                        context.Response.StatusCode = (int)statusCode;
                        context.Response.ContentType = "application/json";

                        var responseJson = JsonConvert.SerializeObject(responseModel, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                        await context.Response.WriteAsync(responseJson);
                    });
            });

            // app.UseMiddleware(typeof(HangfireMiddleware));

            app.UseDefaultFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Configuration.GetValue<string>("QuarterlyReportPath"))
            });

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Remsng Client API");
            });

            app.UseMvc();
        }
    }
}
