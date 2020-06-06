using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenBr.Endereco.Worker.Config;
using OpenBr.Endereco.Worker.Jobs;
using OpenBr.Endereco.Worker.Schedule;

namespace OpenBr.Endereco.Worker
{
    public class Program
    {

        private static IConfigurationBuilder builder;
        private static IConfiguration configuration;
        private static WorkerConfig workerConfig;

        public static void Main(string[] args)
        {

            string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configuration = builder.Build();
            workerConfig = configuration.Get<WorkerConfig>();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddHostedService<Worker>();
                    services.AddCronJob<MyCronJob>(o =>
                    {
                        o.TimeZoneInfo = TimeZoneInfo.Utc;
                        o.CronExpression = workerConfig.Scheduler.Cron;
                    });
                });
    }
}
