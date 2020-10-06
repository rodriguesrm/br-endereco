using OpenBr.Endereco.Business.Documents;
using OpenBr.Endereco.Web.Api.Model;

namespace OpenBr.Endereco.Web.Api.Extesions
{

    /// <summary>
    /// Extensões para objeto EnderecoResponse
    /// </summary>
    public static class EnderecoResponseExtensions
    {

        /// <summary>
        /// Mapeia um objeto cep-document para um objeto cep-response
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
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