using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OpenBr.Endereco.Business.Repositories;

namespace OpenBr.Endereco.Business.Documents
{

    /// <summary>
    /// Objeto abstrato base de documentos de IEntity
    /// </summary>
    public abstract class DocumentBase : IEntity
    {

        /// <inheritdoc/>
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

    }
}
