using System.Collections.Generic;

namespace OpenBr.Endereco.Web.Api.Model
{

    /// <summary>
    /// Informações das críticas de validações
    /// </summary>
    public class ValidacaoResult
    {

        /// <summary>
        /// Detalhe das críticas
        /// </summary>
        public IDictionary<string, string[]> Criticas { get; set; }

    }

}
