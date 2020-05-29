using OpenBr.Endereco.Web.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace OpenBr.Endereco.Web.Api.Filters
{

    /// <summary>
    /// Filter de validação das models das requisições
    /// </summary>
    public class ValidateModelFilter : IActionFilter
    {

        ///<inheritdoc/>
        public void OnActionExecuted(ActionExecutedContext ctx) { }

        ///<inheritdoc/>
        public void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (!ctx.ModelState.IsValid)
            {
                ctx.Result = new BadRequestObjectResult(
                        new ValidacaoResult()
                        {
                            Criticas = ctx.ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                        });
            }
        }
    }

}
