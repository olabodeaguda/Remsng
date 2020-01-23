using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RemsNG.Data;
using System;

namespace RemsNG.Extensions
{
    public static class DatabaseConfiguration
    {
        public static void AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<RemsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                 sqlServerOptions => sqlServerOptions.CommandTimeout(15000)));
            services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<DbContext, RemsDbContext>();

        }

        public static void Migrate(IServiceProvider services)
        {
            using (var serviceScope = services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
                context.Database.Migrate();
            }
        }
    }
}
