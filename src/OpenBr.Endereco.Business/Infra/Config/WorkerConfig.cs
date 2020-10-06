namespace OpenBr.Endereco.Business.Infra.Config
{

    /// <summary>
    /// Objeto de DTO das configurações do worker
    /// </summary>
    public class WorkerConfig
    {

        /// <summary>
        /// Configurações do Scheduler
        /// </summary>
        public SchedulerConfig Scheduler { get; set; }

        /// <summary>
        /// Configurações dos Correios
        /// </summary>
        public CorreiosConfig Correios { get; set; }

    }
}
