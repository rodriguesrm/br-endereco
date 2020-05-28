using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Br.Endereco.Business.HealthCheck
{

    /// <summary>
    /// Executa operação de ping em um determindo host
    /// </summary>
    public class PingHealthCheck : IHealthCheck
    {

        #region Objetos/Variáveis Locais

        private readonly string _host;
        private readonly int _timeout;

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância de PingHealthCheck
        /// </summary>
        /// <param name="host">Host de destino</param>
        /// <param name="timeout">Tempo máximo de espera</param>
        public PingHealthCheck(string host, int timeout)
        {
            _host = host;
            _timeout = timeout;
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Executa uma operação de check (ping) para o host informado
        /// </summary>
        /// <param name="context">Contexto de HealthCheck</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(_host, _timeout);
                HealthCheckResult result;
                if (reply.Status != IPStatus.Success)
                {
                    result = HealthCheckResult.Unhealthy($"Ping check status [{ reply.Status }]. Host {_host} did not respond within {_timeout} ms.");
                }
                else
                {
                    if (reply.RoundtripTime >= _timeout)
                    {
                        result = HealthCheckResult.Degraded($"Ping check for {_host} takes too long to respond. Expected {_timeout} ms but responded in {reply.RoundtripTime} ms.");
                    }
                    else
                    {
                        result = HealthCheckResult.Healthy($"Ping check for {_host} is ok.");
                    }
                }

                return result;
            }
            catch
            {
                return HealthCheckResult.Unhealthy($"Error when trying to check ping for {_host}.");
            }

        }

        #endregion

    }

}