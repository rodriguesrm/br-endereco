using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using OpenBr.Endereco.Business.Documents;
using OpenBr.Endereco.Business.Repositories;
using OpenBr.Endereco.Web.Api.Extesions;
using OpenBr.Endereco.Web.Api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
        /// <param name="cepRepository">Repositóro de cep</param>
        /// <param name="buscaRepository">Repositório de busca de cep nos correios</param>
        /// <param name="cep">Cep a ser consultado</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <response code="200">Sucesso na busca, retorno dos dados do cep</response>
        /// <response code="400">Requisição inválida, verifique as mensagens</response>
        /// <response code="404">Cep não localizado</response>
        [HttpGet("cep/{cep}")]
        [ProducesResponseType(typeof(EnderecoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidacaoResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterCep(
            [FromServices] ICepRepository cepRepository, 
            [FromServices] IBuscaRepository buscaRepository,
            [RegularExpression("(^[0-9]{8}$)", ErrorMessage = "O CEP deve possuir 8 dígitos numéricos")][FromRoute] string cep,
            CancellationToken cancellationToken = default)
        {

            CepDocument doc = await cepRepository.ObterPorCep(cep, cancellationToken);
            if (doc == null)
            {
                await buscaRepository.RegistraBuscaCep(cep);
                return NotFound($"CEP '{cep}' não encontrado");
            }
            else
            {
                return Ok(doc.Map());
            }
        }

    }

}