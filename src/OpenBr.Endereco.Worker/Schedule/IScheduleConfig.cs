using System;

namespace OpenBr.Endereco.Worker.Schedule
{

    /// <summary>
    /// Interface de configurações de agendamento
    /// </summary>
    /// <typeparam name="TJob">Tipo de job a ser executado</typeparam>
    public interface IScheduleConfig<TJob>
    {

        /// <summary>
        /// Expressão cron de agendamento
        /// </summary>
        string CronExpression { get; set; }
        
        /// <summary>
        /// Informações de timezone
        /// </summary>
        TimeZoneInfo TimeZoneInfo { get; set; }

    }
}
