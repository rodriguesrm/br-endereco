using OpenBr.Endereco.Business.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBr.Endereco.Business.Repositories
{

    /// <summary>
    /// Interface de repositório de Cep
    /// </summary>
    public interface ICepRepository : IRepository<CepDocument>
    {

        /// <summary>
        /// Obter documento pelo cep
        /// </summary>
        /// <param name="cep">Cep a ser localizado</param>
        /// <param name="cancellationToken">Token de cancelamento da operação</param>
        Task<CepDocument> ObterPorCep(string cep, CancellationToken cancellationToken = default);

    }
}
