using OpenBr.Endereco.Business.Documents;
using System;

namespace OpenBr.Endereco.Web.Api.Model
{

    public class EnderecoResponse
    {

        /// <summary>
        /// Número do cep
        /// </summary>
        public string Cep { get; set; }

        /// <summary>
        /// Tipo de logradouro
        /// </summary>
        public string TipoLogradouro { get; set; }

        /// <summary>
        /// Nome do logradouro
        /// </summary>
        public string Logradouro { get; set; }

        /// <summary>
        /// Nome do bairro
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// Nome da cidade
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// Unidade da federação (estado)
        /// </summary>
        public string Uf { get; set; }

        /// <summary>
        /// Código Ibge da cidade
        /// </summary>
        public string CodigoIbge { get; set; }

    }

}