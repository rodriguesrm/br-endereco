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
    /// Classe de inicialização da aplicação
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Propriedade das configurações injetadas
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Cria uma nova instância da aplicação
        /// </summary>
        /// <param name="configuration">Injeção das configurações</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuração dos serviços
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
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
                        Title = "OpenBrasil Endereços",
                        Version = "v1",
                        Description = "Api de busca de Endereços (CEP & Logradouro)",
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

            // Serviços de injeção da aplicação
            services.AddApplicationService(Configuration);
            services.AddMiddlewareLoggingOption(Configuration);

            services
                .AddControllers(opt => GlobalFilters.Configure(opt))
                .ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true);

        }

        /// <summary>
        /// Esse método é chamado pelo 'runtime'. Use-o para configurar o piplene das requisições HTTP
        /// </summary>
        /// <param name="app">Builder da aplicação</param>
        /// <param name="env">Informações do ambiente do host da aplicação web</param>
        /// <param name="serviceProvider">Provedor de serviços de injeção de dependência</param>
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenBrasil Endereços v1.0");
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

            // Criação dos objetos de banco de dados
            IDbDocumentCollectionCreator criador = serviceProvider.GetService<IDbDocumentCollectionCreator>();
            criador.Criar().Wait();

        }

    }
}
