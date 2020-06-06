using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OpenBr.Endereco.Business.Infra.MongoDb;
using OpenBr.Endereco.Business.Repositories;
using OpenBr.Endereco.Business.Infra.Config;
using OpenBr.Endereco.Business.Correios;

namespace OpenBr.Endereco.Worker.IoC
{

    /// <summary>
    /// Injetor de serviços de dependência
    /// </summary>
    public static class DependencyInjection
    {

        /// <summary>
        /// Registrador de serviço de injeção de dependência
        /// </summary>
        /// <param name="services">Coleção de serviços de injeção</param>
        /// <param name="configuration">Coleção de configurações</param>
        public static IServiceCollection AddWorkerService(this IServiceCollection services, IConfiguration configuration)
        {

            // Configurações
            services.AddScoped<IConfigurationBuilder, ConfigurationBuilder>();
            services.Configure<ApplicationConfig>(configuration.Bind);
            services.Configure<WorkerConfig>(configuration.Bind);

            //MongoDB
            services.AddSingleton<MongoDatabaseProvider>();
            services.AddSingleton(s => s.GetService<MongoDatabaseProvider>().GetDatabase());

            // Repositorios
            services.AddSingleton<ICepRepository, CepRepository>();
            services.AddSingleton<IBuscaRepository, BuscaRepository>();

            services.AddSingleton<IIntegracaoCorreios, IntegracaoCorreios>();

            return services;

        }


    }

}
