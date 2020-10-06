using Microsoft.Extensions.DependencyInjection;
using OpenBr.Endereco.Worker.Jobs;
using System;

namespace OpenBr.Endereco.Worker.Schedule
{

    /// <summary>
    /// Extensão de adição de jobs ao serviço de container
    /// </summary>
    public static class ScheduledServiceExtensions
    {
        /// <summary>
        /// Adiciona o job ao container de execução
        /// </summary>
        /// <typeparam name="TJob">Tipo de job a ser executado</typeparam>
        /// <param name="services">Coleção de serviços do container</param>
        /// <param name="options">Opções do agendamento</param>
        /// <returns></returns>
        public static IServiceCollection AddCronJob<TJob>(this IServiceCollection services, Action<IScheduleConfig<TJob>> options) where TJob : CronJobService<TJob>
        {

            if (options == null)
                throw new ArgumentNullException(nameof(options), @"Forneça configurações de agendamento.");

            ScheduleConfig<TJob> config = new ScheduleConfig<TJob>();
            
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
                throw new ArgumentNullException(nameof(ScheduleConfig<TJob>.CronExpression), @"Expressão Cron vazia não é permitida.");

            services.AddSingleton<IScheduleConfig<TJob>>(config);
            services.AddHostedService<TJob>();

            return services;

        }
    }
}
