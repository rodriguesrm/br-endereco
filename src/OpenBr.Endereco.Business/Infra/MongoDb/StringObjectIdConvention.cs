using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;

namespace OpenBr.Endereco.Business.Infra.MongoDb
{

    /// <summary>
    /// Contém a conversão do ObjectId<->String entre Application-MongoDb
    /// </summary>
    public class StringObjectIdConvention : ConventionBase, IPostProcessingConvention
    {

        ///<inheritdoc/>
        public void PostProcess(BsonClassMap classMap)
        {

            BsonMemberMap idMapeado = classMap.IdMemberMap;
            if (idMapeado != null && idMapeado.MemberName == "Id" && idMapeado.MemberType == typeof(string))
            {
                idMapeado.SetIdGenerator(new StringObjectIdGenerator());
            }

        }
    }

}
