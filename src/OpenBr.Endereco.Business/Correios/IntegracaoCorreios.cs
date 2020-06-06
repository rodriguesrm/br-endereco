using Microsoft.Extensions.Options;
using OpenBr.Endereco.Business.Documents;
using OpenBr.Endereco.Business.Infra.Config;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenBr.Endereco.Business.Correios
{

    /// <summary>
    /// Provê uma integração com o serviço SOAP gratuito dos correios
    /// </summary>
    public class IntegracaoCorreios : IIntegracaoCorreios
    {

        #region Objetos/Variáveis Locais

        private readonly WorkerConfig _config;

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância do objeto
        /// </summary>
        public IntegracaoCorreios(IOptions<WorkerConfig> options)
        {
            _config = options?.Value;
        }

        #endregion

        #region Métodos Locais

        /// <summary>
        /// Converte a string de retorno dos correios em uma instância de CepDocument
        /// </summary>
        /// <param name="retorno">Expressão de retorno dos correios</param>
        private CepDocument ConverteParaEndereco(string retorno)
        {

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(retorno);

            XmlNode tagEndereco = xml.GetElementsByTagName("return")?[0];

            if (tagEndereco == null)
                return null;

            string tipoLogr = tagEndereco.SelectSingleNode("end")?.InnerText?.Split(' ')[0];

            return new CepDocument()
            {
                Cep = tagEndereco.SelectSingleNode("cep").InnerText,
                TipoLogradouro = tipoLogr,
                Logradouro = !string.IsNullOrWhiteSpace(tipoLogr) ? tagEndereco.SelectSingleNode("end")?.InnerText?.Replace(tipoLogr, "").Trim() : string.Empty,
                Bairro = tagEndereco.SelectSingleNode("bairro").InnerText,
                Cidade = tagEndereco.SelectSingleNode("cidade").InnerText,
                Uf = tagEndereco.SelectSingleNode("uf").InnerText,
            };

        }

        #endregion

        #region Métodos Públicos

        ///<inheritdoc/>
        public async Task<CepDocument> BuscaEndereco(string cep)
        {

            StringContent envelope =
                new StringContent
                (
                    $@"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" 
                        xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" 
                        xmlns:xsd=""http://www.w3.org/1999/XMLSchema""
                        xmlns:cli=""http://cliente.bean.master.sigep.bsb.correios.com.br/"">
                            <SOAP-ENV:Body>
                                <cli:consultaCEP>
                                    <cep>{cep}</cep>
                                </cli:consultaCEP>
                            </SOAP-ENV:Body>
                    </SOAP-ENV:Envelope>", Encoding.UTF8, "application/xml"
                );

            string uri = _config.Correios.UrlService;

            HttpResponseMessage resp = await new HttpClient().PostAsync(uri, envelope);

            string retorno = resp.Content.ReadAsStringAsync().Result;

            if (retorno == null || retorno.Contains("soap:Fault"))
                return null;

            return ConverteParaEndereco(retorno);

        }

        #endregion

    }

}
