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
                RecurringJob.AddOrUpdate(() => runDemandNoticeService.GenerateBulkDemandNotice(), Cron.MinuteInterval(2));
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Scheduler exception");
            }
        }
    }
}
