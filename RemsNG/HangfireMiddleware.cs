using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Interfaces.Managers;
using System;
using System.Threading.Tasks;

namespace RemsNG
{
    public class HangfireMiddleware
    {
        private readonly RequestDelegate next;
        private static ILogger logger;
        public HangfireMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ILoggerFactory loggerFactory,
            IRunDemandNoticeManager runDemandNoticeService)
        {
            logger = loggerFactory.CreateLogger("Recurring job scheduler");

            try
            {
                //RecurringJob.AddOrUpdate(() => runDemandNoticeService.RegisterTaxpayer(), Cron.MinuteInterval(2));
                //RecurringJob.AddOrUpdate(() => runDemandNoticeService.GenerateBulkDemandNotice(), Cron.MinuteInterval(2));
                
                // RecurringJob.AddOrUpdate(() => runDemandNoticeService.TaxpayerPenalty(), Cron.MinuteInterval(2));
                //RecurringJob.AddOrUpdate("Sync Upstream", () => syncService.SyncUp(), Cron.MinuteInterval(4));
                //RecurringJob.AddOrUpdate("Sync Downstream", () => syncService.SyncDown(), Cron.MinuteInterval(4));
                //RecurringJob.AddOrUpdate("Update demand notice table", () => runDemandNoticeService.ReconcileDemandNotice(), Cron.MinuteInterval(4));
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Scheduler exception");
            }
        }
    }
}
