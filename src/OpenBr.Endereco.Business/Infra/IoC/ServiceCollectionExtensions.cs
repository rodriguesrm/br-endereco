using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenBr.Endereco.Business.Infra.IoC
{

    /// <summary>
    /// Extensão de coleção de serviços de injeção de dependência
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registra todos os tipos
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="lifetime">Tempo de vida</param>
        /// <param name="assemblies">Lista dos assemblies</param>
        public static void RegisterAllTypes<T>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
        {
            IEnumerable<TypeInfo> tipos = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
            foreach (TypeInfo type in tipos)
            {
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
            }
        }

    }

}