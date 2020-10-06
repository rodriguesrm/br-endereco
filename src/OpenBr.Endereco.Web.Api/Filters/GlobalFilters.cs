using Microsoft.AspNetCore.Mvc;

namespace OpenBr.Endereco.Web.Api.Filters
{

    /// <summary>
    /// Filter global da aplicação
    /// </summary>
    public static class GlobalFilters
    {
        /// <summary>
        /// Configura o filtro
        /// </summary>
        /// <param name="opt">Objeto mvc-options</param>
        public static void Configure(MvcOptions opt)
        {
            opt.Filters.Add<ValidateModelFilter>();
        }
    }

}
