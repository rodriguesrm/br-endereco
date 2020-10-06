using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenBr.Endereco.Worker.Schedule;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBr.Endereco.Worker.Jobs
{
    public class MyCronJob : CronJobService<MyCronJob>
    {

        private readonly ILogger<MyCronJob> _logger;

        public MyCronJob(IScheduleConfig<MyCronJob> config, ILogger<MyCronJob> logger) : this(config, logger, true) { }

        public MyCronJob(IScheduleConfig<MyCronJob> config, ILogger<MyCronJob> logger, bool runImmediately)
            : base(config.CronExpression, config.TimeZoneInfo, logger, runImmediately)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob is working. [Performed {_executionCounter} time{(_executionCounter == 1 ? "" : "s")}]");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob is stopping.");
            return base.StopAsync(cancellationToken);
        }

    }
}
