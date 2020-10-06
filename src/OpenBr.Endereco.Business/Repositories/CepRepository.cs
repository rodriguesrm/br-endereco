using OpenBr.Endereco.Business.Documents;
using OpenBr.Endereco.Business.Helpers;
using OpenBr.Endereco.Business.Infra.MongoDb;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBr.Endereco.Business.Repositories
{

    public class CepRepository : RepositoryBase<CepDocument>, ICepRepository, IDocumentCollectionCreator
    {

        #region Objetos/Variáveis locais

        #endregion

        #region Construtores

        ///<inheritdoc/>
        public CepRepository(IMongoDatabase mongoDb) : base(mongoDb, "cep")
        {
        }

        #endregion

        #region Overrides

        ///<inheritdoc/>
        protected override UpdateDefinition<CepDocument> MapearCamposParaAtualizacao(CepDocument document)
        {
            UpdateDefinition<CepDocument> update = Builders<CepDocument>.Update
                .Set(x => x.Cep, document.Cep)
                .Set(x => x.TipoLogradouro, document.TipoLogradouro)
                .Set(x => x.Logradouro, document.Logradouro)
                .Set(x => x.Bairro, document.Bairro)
                .Set(x => x.Cidade, document.Cidade)
                .Set(x => x.Uf, document.Uf)
                .Set(x => x.CodigoIbge, document.CodigoIbge);
            return update;
        }

        #endregion

        #region Métodos Públicos

        ///<inheritdoc/>
        public async Task<CepDocument> ObterPorCep(string cep, CancellationToken cancellationToken = default)
        {
            FilterDefinition<CepDocument> fd = Builders<CepDocument>.Filter.Eq(nameof(CepDocument.Cep).ToCamelCase(), cep);
            return await Collection.Find(fd).FirstOrDefaultAsync(cancellationToken);
        }

        ///<inheritdoc/>
        public async Task CriarColecaoNaBaseAsync()
        {
            await CriarColecaoAsync();
            await CriarIndiceAsync(c => c.Cep, nameof(CepDocument.Cep).ToCamelCase(), true);
            await CriarIndiceAsync(c => c.Logradouro, nameof(CepDocument.Logradouro).ToCamelCase());
            await CriarIndiceAsync(c => c.CodigoIbge, nameof(CepDocument.CodigoIbge).ToCamelCase());
        }

        #endregion

    }
}