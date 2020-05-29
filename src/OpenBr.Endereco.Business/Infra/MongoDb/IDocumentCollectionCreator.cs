using System.Threading.Tasks;

namespace OpenBr.Endereco.Business.Infra.MongoDb
{

    /// <summary>
    /// Interface que criação da coleção na base MongoDb
    /// </summary>
    public interface IDocumentCollectionCreator
    {

        /// <summary>
        /// Cria a coleção
        /// </summary>
        Task CriarColecaoNaBaseAsync();

    }
}
