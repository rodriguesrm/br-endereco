using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq;
using System.Linq.Expressions;

namespace OpenBr.Endereco.Business.Repositories
{

    public abstract class RepositoryBase<TDocument> : IRepository<TDocument>
        where TDocument : IEntity
    {

        #region Objetos/Variáveis Locais

        protected string _nomeColecao;
        protected IMongoDatabase _db;

        /// <summary>
        /// Coleção do banco
        /// </summary>
        protected IMongoCollection<TDocument> Collection => _db.GetCollection<TDocument>(_nomeColecao);

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância do repositório
        /// </summary>
        /// <param name="mongoDb">Instância do Banco de dados Mongo</param>
        /// <param name="nomeColecao">Nome da coleção</param>
        public RepositoryBase(IMongoDatabase mongoDb, string nomeColecao)
        {
            _nomeColecao = nomeColecao;
            _db = mongoDb;
        }

        #endregion

        #region Métodos abstratos

        /// <summary>
        /// Mapear os campos a serem atualizados
        /// </summary>
        /// <param name="document">Entidade do documento</param>
        protected abstract UpdateDefinition<TDocument> MapearCamposParaAtualizacao(TDocument document);

        #endregion

        #region Métodos Públicos

        /// <inheritdoc/>
        public virtual async Task<TDocument> ObterPorId(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<TDocument> fd = Builders<TDocument>.Filter.Eq("_id", id);
            return await Collection.Find(fd).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TDocument>> Listar(CancellationToken cancellationToken = default)
            => await Collection.AsQueryable().ToListAsync(cancellationToken);

        /// <inheritdoc/>
        public virtual async Task Adicionar(TDocument document, CancellationToken cancellationToken = default)
            =>  await Collection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken);

        /// <inheritdoc/>
        public virtual async Task Editar(TDocument document, CancellationToken cancellationToken = default)
        {
            FilterDefinition<TDocument> fd = Builders<TDocument>.Filter.Eq("_id", document.Id);
            UpdateDefinition<TDocument> update = MapearCamposParaAtualizacao(document);
            await Collection.UpdateOneAsync(fd, update, null, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task Remover(TDocument document, CancellationToken cancellationToken = default)
        {
            FilterDefinition<TDocument> fd = Builders<TDocument>.Filter.Eq("_id", document.Id);
            await Collection.DeleteOneAsync(fd, cancellationToken);
        }

        #endregion

        #region ICollectionCreator

        /// <summary>
        /// Verifia se uma coleção existe
        /// </summary>
        protected async Task<bool> ExisteColexaoAsync()
        {

            IAsyncCursor<BsonDocument> colecoes = await _db.ListCollectionsAsync();
            bool result = false;

            if (colecoes != null)
                await colecoes.ForEachAsync(document => result = document["name"].AsString.Equals(_nomeColecao));

            return result;

        }

        /// <summary>
        /// Obtém a coleção
        /// </summary>
        protected IMongoCollection<TDocument> ColecaoMongo => _db.GetCollection<TDocument>(_nomeColecao);

        /// <summary>
        /// Cria a coleção se não existir
        /// </summary>
        protected Task<IMongoCollection<TDocument>> CriarColecaoAsync()
            => CriarColecaoAsync(false, null, null);

        /// <summary>
        /// Cria a coleção se não existir
        /// </summary>
        /// <param name="limitada">Indica se a coleção será do tipo limitada ("capped")</param>
        /// <param name="tamanhoMaximo">Tamanho máximo da coleção</param>
        /// <param name="limiteDocumentos">Número máxio de documentos</param>
        protected async Task<IMongoCollection<TDocument>> CriarColecaoAsync(bool limitada, long? tamanhoMaximo, long? limiteDocumentos)
        {

            if (!(await ExisteColexaoAsync()))
            {

                if (limitada && tamanhoMaximo > 0)
                {

                    CreateCollectionOptions options = new CreateCollectionOptions
                    {
                        Capped = limitada,
                        MaxSize = tamanhoMaximo
                    };

                    if (limiteDocumentos > 0)
                        options.MaxDocuments = limiteDocumentos;

                    await _db.CreateCollectionAsync(_nomeColecao, options);

                }
                else
                {
                    await _db.CreateCollectionAsync(_nomeColecao);
                }

            }

            return ColecaoMongo;

        }


        /// <summary>
        /// Verifica se um índice existe
        /// </summary>
        /// <param name="colecao">Nome da coleção</param>
        /// <param name="nomesIndices">Lista dos nomes dos índices a serem verificados</param>
        protected async Task<bool> ExisteIndiceAsync(IMongoCollection<TDocument> colecao, params string[] nomesIndices)
        {
            using (IAsyncCursor<BsonDocument> cursor = await colecao.Indexes.ListAsync())
            {
                IEnumerable<BsonDocument> indices = await cursor.ToListAsync();
                foreach (BsonDocument ix in indices)
                {
                    BsonDocument currentIndex = ix.GetValue("key", null).ToBsonDocument();
                    if (currentIndex == null) continue;
                    bool result = nomesIndices.Any(checkIndex => (currentIndex.GetValue(checkIndex, 0).ToBoolean()));
                    if (result) return true;
                }
            }

            return false;

        }

        /// <summary>
        /// Cria um índice, se não existir
        /// </summary>
        /// <param name="campo">Nome do campo</param>
        /// <param name="nomeIndice">Nome do índice</param>
        protected Task CriarIndiceAsync(Expression<Func<TDocument, object>> campo, string nomeIndice)
            => CriarIndiceAsync(campo, nomeIndice, false, null);

        /// <summary>
        /// Cria um índice, se não existir
        /// </summary>
        /// <param name="campo">Nome do campo</param>
        /// <param name="nomeIndice">Nome do índice</param>
        /// <param name="exclusivo">Indica se o índice é único/exclusivo</param>
        protected Task CriarIndiceAsync(Expression<Func<TDocument, object>> campo, string nomeIndice, bool exclusivo)
        {
            return CriarIndiceAsync(campo, nomeIndice, exclusivo, null);
        }

        /// <summary>
        /// Cria um índice se não existir
        /// </summary>
        /// <param name="nomeINdice">Nome do índice</param>
        /// <param name="exclusivo">Indica se o índice é único/exclusivo</param>
        /// <param name="tempoVida">Tempo de vida do documento</param>
        /// <param name="campos">Lista de campos</param>
        protected async Task CriarIndiceAsync(string nomeINdice, bool exclusivo, int? tempoVida, params Expression<Func<TDocument, object>>[] campos)
        {
            
            IMongoCollection<TDocument> colecao = ColecaoMongo;

            if (colecao != null)
            {
                if (!await ExisteIndiceAsync(colecao, nomeINdice))
                {
                    IndexKeysDefinition<TDocument>[] chavesIndice = new IndexKeysDefinition<TDocument>[campos.Length];

                    for (int i = 0; i < campos.Length; i++)
                    {
                        chavesIndice[i] = Builders<TDocument>.IndexKeys.Ascending(campos[i]);
                    }

                    CreateIndexOptions parametrosIndice;

                    if (tempoVida.HasValue)
                        parametrosIndice = new CreateIndexOptions { Name = nomeINdice, Unique = exclusivo, ExpireAfter = TimeSpan.FromDays(tempoVida.Value) };
                    else
                        parametrosIndice = new CreateIndexOptions { Name = nomeINdice, Unique = exclusivo };

                    IndexKeysDefinition<TDocument> definicaoIndice = Builders<TDocument>.IndexKeys.Combine(chavesIndice);
                    CreateIndexModel<TDocument> modeloIndice = new CreateIndexModel<TDocument>(definicaoIndice, parametrosIndice);
                    await colecao.Indexes.CreateOneAsync(modeloIndice);

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campo">Nome do campo</param>
        /// <param name="nomeIndice">Nome do índice</param>
        /// <param name="exclusivo">Indica se o índice é único/exclusivo</param>
        /// <param name="tempoVida">Tempo de vida do documento</param>
        protected async Task CriarIndiceAsync(Expression<Func<TDocument, object>> campo, string nomeIndice, bool exclusivo, int? tempoVida)
        {
            IMongoCollection<TDocument> colecao = ColecaoMongo;

            if (colecao != null)
            {
                if (!await ExisteIndiceAsync(colecao, nomeIndice))
                {

                    TimeSpan? tsDias = null;
                    if (tempoVida.HasValue)
                        tsDias = TimeSpan.FromDays(tempoVida.Value);

                    CreateIndexOptions parametrosIndice = new CreateIndexOptions { Name = nomeIndice, Unique = exclusivo, ExpireAfter = tsDias };
                    CreateIndexModel<TDocument> modeloIndice = new CreateIndexModel<TDocument>(Builders<TDocument>.IndexKeys.Ascending(campo), parametrosIndice);
                    await colecao.Indexes.CreateOneAsync(modeloIndice);

                }
            }
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        /// Para detectar chamadas redundantes
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// Libera recursos
        /// </summary>
        /// <param name="disposing">Flag indicando execução do 'dispose'</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //object?.Dispose();
                }

                _db = null;
                _nomeColecao = null;

                disposedValue = true;
            }
        }

        /// <summary>
        /// Destrutor do repositório
        /// </summary>
        ~RepositoryBase()
        {
          Dispose(false);
        }

        /// <summary>
        /// Libera os recursos do objeto
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion


    }

}
