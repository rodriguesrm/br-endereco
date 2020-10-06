using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using OpenBr.Endereco.Business.Infra.Config;

namespace OpenBr.Endereco.Business.Infra.MongoDb
{

    /// <summary>
    /// Provê uma conexão com a base de dados MongoDb
    /// </summary>
    public class MongoDatabaseProvider
    {

        #region Objetos/Variáveis Locais

        protected ApplicationConfig _appConfig;

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância do provider
        /// </summary>
        /// <param name="configuration">Coleção de configurações</param>
        public MongoDatabaseProvider(IOptions<ApplicationConfig> appConfig)
        {
            _appConfig = appConfig.Value;
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
            string nomeBd = MongoUrl.Create(_appConfig.ConnectionStrings.MongoDb).DatabaseName;
            MongoClient dbClient = new MongoClient(_appConfig.ConnectionStrings.MongoDb);
            return dbClient.GetDatabase(nomeBd);
        }

        #endregion

    }
}
