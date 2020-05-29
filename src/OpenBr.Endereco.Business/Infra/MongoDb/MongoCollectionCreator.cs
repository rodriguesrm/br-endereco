using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenBr.Endereco.Business.Infra.MongoDb
{

    /// <summary>
    /// Criador de objetos de banco de dados MongoDb (Collections, Indexes, etc)
    /// </summary>
    public class MongoCollectionCreator : IDbDocumentCollectionCreator
    {

        #region Objetos/Variáveis Locais

        private readonly IEnumerable<IDocumentCollectionCreator> _criadores;

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância de MongoCollectionCreator
        /// </summary>
        /// <param name="criadores">Lista dos repositórios criadores</param>
        public MongoCollectionCreator(IEnumerable<IDocumentCollectionCreator> criadores)
        {
            _criadores = criadores;
        }

        #endregion

        ///<inheritdoc/>
        public async Task Criar()
        {
            await Task.WhenAll(_criadores.Select(c => c.CriarColecaoNaBaseAsync()));
        }

    }

}
