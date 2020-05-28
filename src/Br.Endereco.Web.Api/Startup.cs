using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Br.Endereco.Business.HealthCheck;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Br.Endereco.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region HealthCheck

            services.AddHealthChecks()
                    //.AddCheck("Google Ping", new PingHealthCheck("www.google.com", 100))
                    //.AddCheck("Bing Ping", new PingHealthCheck("www.bing.com", 100))
                    .AddUrlGroup(
                                new Uri("http://google.com"),
                                name: "Acesso ao Google",
                                failureStatus: HealthStatus.Degraded)
                    .AddUrlGroup(
                                new Uri("https://www.bing.com"),
                                name: "Acesso ao Bing",
                                failureStatus: HealthStatus.Degraded)
                    .AddMongoDb(
                                mongodbConnectionString: Configuration.GetValue<string>("ConnectionStrings:MongoDb"),
                                name: "MongoDB",
                                failureStatus: HealthStatus.Unhealthy,
                                timeout: TimeSpan.FromSeconds(15),
                                tags: new string[] { "mongodb" });

            services.AddHealthChecksUI();

            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region HealthCheck

            app
                .UseHealthChecks("/selfcheck", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                })
                .UseHealthChecksUI()
                ;

            #endregion

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
