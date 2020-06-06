using OpenBr.Endereco.Business.Enums;
using System;

namespace OpenBr.Endereco.Business.Documents
{

    /// <summary>
    /// Documento de buca de ceps no webservice publico dos correios
    /// </summary>
    public class BuscaDocument : DocumentBase
    {

        /// <summary>
        /// Cria uma nova instância do documento
        /// </summary>
        public BuscaDocument()
        {
            DataInclusao = DateTime.Now;
        }

        /// <summary>
        /// Cep a ser buscado
        /// </summary>
        public string Cep { get; set; }

        /// <summary>
        /// Indica se a busca foi finalizada
        /// </summary>
        public BuscaStatus Status { get; set; }

        /// <summary>
        /// Data de inclusão do registro
        /// </summary>
        public DateTime DataInclusao { get; set; }

        /// <summary>
        /// Data da última busca
        /// </summary>
        public DateTime? DataUltimaBusca { get; set; }

        /// <summary>
        /// Data da finalização do processo
        /// </summary>
        public DateTime? DataFinalizacao { get; set; }

        /// <summary>
        /// Dados encontrados na busca finalizada
        /// </summary>
        public CepDocument Resultado { get; set; }

        /// <summary>
        /// Quantidade de buscas realizadas
        /// </summary>
        public int BuscasRealizadas { get; set; }


    }
}
