using OpenBr.Endereco.Business.Documents;
using OpenBr.Endereco.Business.Helpers;
using OpenBr.Endereco.Business.Infra.MongoDb;
using MongoDB.Driver;
using System.Threading.Tasks;
using System;
using System.Threading;
using OpenBr.Endereco.Business.Enums;

namespace OpenBr.Endereco.Business.Repositories
{

    public class BuscaRepository : RepositoryBase<BuscaDocument>, IBuscaRepository, IDocumentCollectionCreator
    {

        #region Objetos/Variáveis locais

        #endregion

        #region Construtores

        ///<inheritdoc/>
        public BuscaRepository(IMongoDatabase mongoDb) : base(mongoDb, "busca")
        {
        }

        #endregion

        #region Overrides

        ///<inheritdoc/>
        protected override UpdateDefinition<BuscaDocument> MapearCamposParaAtualizacao(BuscaDocument document)
        {

            UpdateDefinition<BuscaDocument> update = Builders<BuscaDocument>.Update
                .Set(x => x.Cep, document.Cep)
                .Set(x => x.Status, document.Status)
                .Set(x => x.DataUltimaBusca, document.DataUltimaBusca)
                .Set(x => x.DataFinalizacao, document.DataFinalizacao)
                .Set(x => x.Resultado, document.Resultado)
            ;
            return update;
        }

        #endregion

        #region Métodos Públicos

        /// <inheritdoc/>
        public async Task<BuscaDocument> ObterPorCep(string cep, CancellationToken cancellationToken = default)
        {
            FilterDefinition<BuscaDocument> fd = Builders<BuscaDocument>.Filter.Eq(nameof(BuscaDocument.Cep).ToCamelCase(), cep);
            return await Collection.Find(fd).FirstOrDefaultAsync(cancellationToken);
        }

        ///<inheritdoc/>
        public async Task CriarColecaoNaBaseAsync()
        {
            await CriarColecaoAsync();
            await CriarIndiceAsync(c => c.Cep, nameof(BuscaDocument.Cep).ToCamelCase(), true);
            await CriarIndiceAsync(c => c.Status, nameof(BuscaDocument.Status).ToCamelCase());
            await CriarIndiceAsync(c => c.DataUltimaBusca, nameof(BuscaDocument.DataUltimaBusca).ToCamelCase());
            await CriarIndiceAsync(c => c.DataFinalizacao, nameof(BuscaDocument.DataFinalizacao).ToCamelCase());
        }

        ///<inheritdoc/>
        public async Task RegistraBuscaCep(string cep)
        {

            if (cep.Length == 8)
            {

                BuscaDocument busca = await ObterPorCep(cep);
                if (busca == null)
                {
                    await Adicionar(new BuscaDocument()
                    {
                        Cep = cep,
                        Status = BuscaStatus.Pendente
                    });
                }

            }
        }

        #endregion

    }
}