using System.Threading.Tasks;

namespace OpenBr.Endereco.Business.Infra.MongoDb
{

    /// <summary>
    /// Interface que criação das coleções na base MongoDb
    /// </summary>
    public interface IDbDocumentCollectionCreator
    {

        /// <summary>
        /// Cria as coleções
        /// </summary>
        Task Criar();

    }
}
