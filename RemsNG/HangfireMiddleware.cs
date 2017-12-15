using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task Invoke(HttpContext context, ILoggerFactory loggerFactory, IRunDemandNoticeService runDemandNoticeService)
        {
            logger = loggerFactory.CreateLogger("Recurring job scheduler");

            try
            {
                RecurringJob.AddOrUpdate(() => runDemandNoticeService.RegisterTaxpayer(), Cron.MinuteInterval(6));
                RecurringJob.AddOrUpdate(() => runDemandNoticeService.TaxpayerPenalty(), Cron.MinuteInterval(4));
                RecurringJob.AddOrUpdate(() => runDemandNoticeService.GenerateBulkDemandNotice(), Cron.MinuteInterval(3));
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Scheduler exception");
            }
        }
    }
}
