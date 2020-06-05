using OpenBr.Endereco.Business.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBr.Endereco.Business.Repositories
{

    /// <summary>
    /// Interface de repositório de Buscas
    /// </summary>
    public interface IBuscaRepository : IRepository<BuscaDocument>
    {

        /// <summary>
        /// Realizar uma busca pelo cep
        /// </summary>
        /// <param name="cep">Cep a ser buscado</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        Task<BuscaDocument> ObterPorCep(string cep, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registra a busca de um cep que não foi encontrado
        /// </summary>
        /// <param name="cep">Cep a ser registrado para busca nos correios</param>
        Task RegistraBuscaCep(string cep);

    }
}
