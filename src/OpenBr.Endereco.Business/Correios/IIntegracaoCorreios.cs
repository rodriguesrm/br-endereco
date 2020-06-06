using OpenBr.Endereco.Business.Documents;
using System.Threading.Tasks;

namespace OpenBr.Endereco.Business.Correios
{

    /// <summary>
    /// Interface de integração com os correios
    /// </summary>
    public interface IIntegracaoCorreios
    {

        /// <summary>
        /// Faz a busca de um cep nos correios
        /// </summary>
        /// <param name="cep">Cep a ser buscado</param>
        Task<CepDocument> BuscaEndereco(string cep);

    }
}