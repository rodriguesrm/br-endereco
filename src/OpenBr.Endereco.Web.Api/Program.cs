using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RSoft.Logs.Extensions;

namespace OpenBr.Endereco.Web.Api
{

    /// <summary>
    /// Objeto de kick-off da aplicação
    /// </summary>
    public class Program
    {

        /// <summary>
        /// Método principal de kick-off
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Construtor do ambiente
        /// </summary>
        /// <param name="args">Lista de argumentos</param>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsoleLogger();
                    logging.AddSeqLogger();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
