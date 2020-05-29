using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using OpenBr.Endereco.Business.Documents;
using OpenBr.Endereco.Business.Repositories;
using OpenBr.Endereco.Web.Api.Extesions;
using OpenBr.Endereco.Web.Api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OpenBr.Endereco.Web.Api.Controllers
{

    /// <summary>
    /// Api de Endereço
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class EnderecoController : ControllerBase
    {

        /// <summary>
        /// Obter os dados do endereço pelo CEP
        /// </summary>
        /// <param name="repository">Repositóro de cep</param>
        /// <param name="cep">Cep a ser consultado</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        [HttpGet("cep/{cep}")]
        [ProducesResponseType(typeof(EnderecoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidacaoResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterCep(
            [FromServices] ICepRepository repository, 
            [RegularExpression("(^[0-9]{8}$)", ErrorMessage = "O CEP deve possuir 8 dígitos numéricos")][FromRoute] string cep,
            CancellationToken cancellationToken = default)
        {

            CepDocument doc = await repository.ObterPorCep(cep, cancellationToken);
            if (doc == null)
                return NotFound("CEP não encontrado");
            else
                return Ok(doc.Map());
        }

    }

}