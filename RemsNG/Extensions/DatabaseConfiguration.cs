using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RemsNG.Data;

namespace RemsNG.Extensions
{
    public static class DatabaseConfiguration
    {
        public static void AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<RemsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<DbContext, RemsDbContext>();
        }
    }
}
