using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace OpenBr.Endereco.Business.Infra.MongoDb
{

    /// <summary>
    /// Provê uma conexão com a base de dados MongoDb
    /// </summary>
    public class MongoDatabaseProvider
    {

        #region Objetos/Variáveis Locais

        protected IConfiguration _configuration;

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância do provider
        /// </summary>
        /// <param name="configuration">Coleção de configurações</param>
        public MongoDatabaseProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region Métodos estáticos

        static MongoDatabaseProvider()
        {
            ConventionPack cp = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new StringObjectIdConvention(),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register("convensoes", cp, t => true);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Obter base de dados
        /// </summary>
        public IMongoDatabase GetDatabase()
        {

            //TODO: RR - Implementar classe geral de leitura de configuraçõeos
            string strConexao = _configuration["ConnectionStrings:MongoDb"];
            string nomeBd = MongoUrl.Create(strConexao).DatabaseName;

            MongoClient dbClient = new MongoClient(strConexao);

            return dbClient.GetDatabase(nomeBd);

        }

        #endregion

    }
}
