using Microsoft.AspNetCore.Mvc;

namespace OpenBr.Endereco.Web.Api.Filters
{

    /// <summary>
    /// Filter global da aplicação
    /// </summary>
    public static class GlobalFilters
    {
        public static void Configure(MvcOptions opt)
        {
            opt.Filters.Add<ValidateModelFilter>();
        }
    }

}
