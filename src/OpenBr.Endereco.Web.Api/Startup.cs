using System;
using System.IO;
using System.Reflection;
using OpenBr.Endereco.Business.Infra.IoC;
using OpenBr.Endereco.Business.Infra.MongoDb;
using OpenBr.Endereco.Web.Api.Filters;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RSoft.Logs.Extensions;
using RSoft.Logs.Middleware;

namespace OpenBr.Endereco.Web.Api
{

    /// <summary>
    /// Classe de inicializa��o da aplica��o
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Propriedade das configura��es injetadas
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Cria uma nova inst�ncia da aplica��o
        /// </summary>
        /// <param name="configuration">Inje��o das configura��es</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configura��o dos servi�os
        /// </summary>
        /// <param name="services">Cole��o de servi�os</param>
        public void ConfigureServices(IServiceCollection services)
        {

            #region HealthCheck

            services.AddHealthChecks()
                    //.AddCheck("Google Ping", new PingHealthCheck("www.google.com", 100))
                    //.AddCheck("Bing Ping", new PingHealthCheck("www.bing.com", 100))
                    .AddUrlGroup(
                                new Uri("https://apps.correios.com.br"),
                                name: "Acesso a Apps dos Correios",
                                failureStatus: HealthStatus.Degraded)
                    .AddMongoDb(
                                mongodbConnectionString: Configuration.GetValue<string>("ConnectionStrings:MongoDb"),
                                name: "MongoDB",
                                failureStatus: HealthStatus.Unhealthy,
                                timeout: TimeSpan.FromSeconds(15),
                                tags: new string[] { "mongodb" });

            #endregion

            #region Swagger

            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "OpenBrasil Endere�os",
                        Version = "v1",
                        Description = "Api de busca de Endere�os (CEP & Logradouro)",
                        Contact = new OpenApiContact
                        {
                            Name = "Rodrigo Rodrigues",
                            Url = new Uri("https://github.com/rodriguesrm")
                        }
                    });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });

            #endregion

            // Servi�os de inje��o da aplica��o
            services.AddApplicationService(Configuration);
            services.AddMiddlewareLoggingOption(Configuration);

            services
                .AddControllers(opt => GlobalFilters.Configure(opt))
                .ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true);

        }

        /// <summary>
        /// Esse m�todo � chamado pelo 'runtime'. Use-o para configurar o piplene das requisi��es HTTP
        /// </summary>
        /// <param name="app">Builder da aplica��o</param>
        /// <param name="env">Informa��es do ambiente do host da aplica��o web</param>
        /// <param name="serviceProvider">Provedor de servi�os de inje��o de depend�ncia</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region HealthCheck

            app
                .UseHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            #endregion

            //app.UseHttpsRedirection();

            app.UseRouting();

            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenBrasil Endere�os v1.0");
                c.RoutePrefix = string.Empty;
                //c.SupportedSubmitMethods(new SubmitMethod[] { });
            });

            #endregion

            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            app.UseStaticFiles();
            app.UseResponseCaching();

            app.UseMiddleware<RequestResponseLogging<Startup>>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Cria��o dos objetos de banco de dados
            IDbDocumentCollectionCreator criador = serviceProvider.GetService<IDbDocumentCollectionCreator>();
            criador.Criar().Wait();

        }

    }
}
