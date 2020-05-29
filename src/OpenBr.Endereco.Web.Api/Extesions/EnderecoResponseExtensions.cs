using OpenBr.Endereco.Business.Documents;
using OpenBr.Endereco.Web.Api.Model;

namespace OpenBr.Endereco.Web.Api.Extesions
{

    /// <summary>
    /// Extensões para objeto EnderecoResponse
    /// </summary>
    public static class EnderecoResponseExtensions
    {

        public static EnderecoResponse Map(this CepDocument doc)
            => new EnderecoResponse()
            {
                Cep = doc.Cep,
                TipoLogradouro = doc.TipoLogradouro,
                Logradouro = doc.Logradouro,
                Bairro = doc.Bairro,
                Cidade = doc.Cidade,
                Uf = doc.Uf,
                CodigoIbge = doc.CodigoIbge
            };

    }

}