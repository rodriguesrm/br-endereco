using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using OpenBr.Endereco.Business.Correios;
using OpenBr.Endereco.Business.Infra.Config;
using OpenBr.Endereco.Worker.Schedule;
using System;
using System.Threading;
using System.Threading.Tasks;
using OpenBr.Endereco.Business.Repositories;
using OpenBr.Endereco.Business.Documents;
using System.Collections.Generic;
using System.Linq;
using OpenBr.Endereco.Business.Enums;

namespace OpenBr.Endereco.Worker.Jobs
{
    public class BuscaEnderecoCorreiosJob : CronJobService<BuscaEnderecoCorreiosJob>
    {

        #region Objetos/Variáveis Locais

        private const string nomeJob = "Busca de Endereço nos Correios";
        private readonly ILogger<BuscaEnderecoCorreiosJob> _logger;
        private readonly IIntegracaoCorreios _correios;
        private readonly IBuscaRepository _buscaRepository;
        private readonly ICepRepository _cepRepository;

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância do job
        /// </summary>
        /// <param name="config">Configuração do Job</param>
        /// <param name="buscaRepository">Repositório de documentos de busca</param>
        /// <param name="cepRepository">Repositório de documentos de cep</param>
        /// <param name="correios">Objeto de integração com os correios</param>
        /// <param name="logger">Objeto de registro de log</param>
        public BuscaEnderecoCorreiosJob
        (
            IScheduleConfig<BuscaEnderecoCorreiosJob> config,
            IBuscaRepository buscaRepository,
            ICepRepository cepRepository,
            IIntegracaoCorreios correios,
            ILogger<BuscaEnderecoCorreiosJob> logger
        ) : this(config, buscaRepository, cepRepository, correios, logger, true) { }

        /// <summary>
        /// Cria uma nova instância do job
        /// </summary>
        /// <param name="config">Configuração do Job</param>
        /// <param name="buscaRepository">Repositório de documentos de busca</param>
        /// <param name="cepRepository">Repositório de documentos de cep</param>
        /// <param name="correios">Objeto de integração com os correios</param>
        /// <param name="logger">Objeto de registro de log</param>
        /// <param name="runImmediately">Indica se uma execução adicional ao iniciar o job deve ser realizada</param>
        public BuscaEnderecoCorreiosJob
        (
            IScheduleConfig<BuscaEnderecoCorreiosJob> config,
            IBuscaRepository buscaRepository,
            ICepRepository cepRepository,
            IIntegracaoCorreios correios,
            ILogger<BuscaEnderecoCorreiosJob> logger,
            bool runImmediately
        ) : base(config.CronExpression, config.TimeZoneInfo, logger, runImmediately)
        {
            _buscaRepository = buscaRepository;
            _cepRepository = cepRepository;
            _logger = logger;
            _correios = correios;
        }

        #endregion

        #region Métodos Locais

        #endregion

        #region Métodos Públicos

        ///<inheritdoc/>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Job '{nomeJob}' iniciado");
            return base.StartAsync(cancellationToken);
        }

        ///<inheritdoc/>
        public override async Task DoWork(CancellationToken cancellationToken)
        {

            try
            {

                IEnumerable<BuscaDocument> buscas = await _buscaRepository.ObterPendentesAsync(cancellationToken);
                if (buscas.Count() == 0)
                {
                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Nenhum registro de busca para processamento");
                }
                else
                {
                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Encontrados {buscas.Count()} registros para processamento");
                    foreach (BuscaDocument b in buscas)
                    {

                        try
                        {

                            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Buscando cep '{b.Cep}'");
                            CepDocument endereco = await _correios.BuscaEndereco(b.Cep);
                            if (endereco == null)
                            {
                                _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Cep '{b.Cep}' não encontrado, nova tentativa será realizada no futuro");
                                b.BuscasRealizadas++;
                                b.DataUltimaBusca = DateTime.Now;

                            }
                            else
                            {
                                b.BuscasRealizadas++;
                                b.DataFinalizacao = DateTime.Now;
                                b.DataUltimaBusca = DateTime.Now;
                                b.Resultado = endereco;
                                _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Cep '{b.Cep}' localizado");
                                if (await _cepRepository.ObterPorCep(b.Cep, cancellationToken) == null)
                                {
                                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Carregando dados do '{b.Cep}' para base");
                                    b.Status = BuscaStatus.Sucesso;
                                    await _cepRepository.Adicionar(endereco, cancellationToken);
                                }
                                else
                                {
                                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Cep '{b.Cep}' localizado, carregando para base");
                                    b.Status = BuscaStatus.Cancelada;
                                }
                            }
                            await _buscaRepository.Editar(b, cancellationToken);

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Falha no processamento do cep '{b.Cep}'");
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha na busca de ceps pendentes");
            }

        }

        ///<inheritdoc/>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Job '{nomeJob}' finalizado");
            return base.StopAsync(cancellationToken);
        }

        #endregion

    }
}
