using Cronos;
using DnsClient.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBr.Endereco.Worker.Jobs
{

    /// <summary>
    /// Objeto abstrato de jobs a serem executados
    /// </summary>
    public abstract class CronJobService<TLog> : IHostedService, IDisposable
        where TLog : class
    {

        #region Objetos/Variáveis Locais

        private System.Timers.Timer _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;
        private readonly bool _runImmediately;
        private readonly ILogger<TLog> _logger;

        protected int _executionCounter;

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância do objeto
        /// </summary>
        /// <param name="cronExpression">Expressão cron de agendamento</param>
        /// <param name="timeZoneInfo">Informações de timezone</param>
        /// <param name="logger">Objeto de log</param>
        protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo, ILogger<TLog> logger) : this(cronExpression, timeZoneInfo, logger, false) { }

        /// <summary>
        /// Cria uma nova instância do objeto
        /// </summary>
        /// <param name="cronExpression">Expressão cron de agendamento</param>
        /// <param name="timeZoneInfo">Informações de timezone</param>
        /// <param name="logger">Objeto de log</param>
        /// <param name="runImmediately">Indica que uma execução deve ser feita imediatamente</param>
        protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo, ILogger<TLog> logger, bool runImmediately)
        {
            _expression = CronExpression.Parse(cronExpression);
            _timeZoneInfo = timeZoneInfo;
            _logger = logger;
            _runImmediately = runImmediately;
            _executionCounter = 0;
        }

        #endregion

        #region Métodos Locais

        /// <summary>
        /// Faz o agendamento da atividade para a próxima execução
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {

            DateTimeOffset? next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {

                    // reset and dispose timer
                    _timer.Dispose();
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        _executionCounter++;
                        await DoWork(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);    // reschedule next
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        #endregion

        #region Métodos Públicos

        ///<inheritdoc/>
        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_runImmediately)
            {
                _executionCounter++;
                await DoWork(cancellationToken);
            }
            await ScheduleJob(cancellationToken);
        }

        /// <summary>
        /// Executa a atividade agendada
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        public virtual async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Do nothing [Performed {_executionCounter} time{(_executionCounter == 1 ? "" : "s")}]");
            await Task.Delay(5000, cancellationToken);  // do the work
        }

        ///<inheritdoc/>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Libera os recursos utilizados
        /// </summary>
        public virtual void Dispose()
        {
            _timer?.Dispose();
        }

        #endregion
    }

}