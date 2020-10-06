using System;

namespace OpenBr.Endereco.Worker.Schedule
{

    /// <summary>
    /// Objeto de configurações de agendamento
    /// </summary>
    /// <typeparam name="TJob">Tipo de job a ser executado</typeparam>
    public class ScheduleConfig<TJob> : IScheduleConfig<TJob>
    {

        ///<inheritdoc/>
        public string CronExpression { get; set; }

        ///<inheritdoc/>
        public TimeZoneInfo TimeZoneInfo { get; set; }

    }

}
