using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBr.Endereco.Business.Repositories
{


    /// <summary>
    /// Interface de repositório
    /// </summary>
    public interface IRepository<TDocument> : IDisposable
        where TDocument : IEntity
    {

        /// <summary>
        /// Obter documento pelo id
        /// </summary>
        /// <param name="id">Id do registro</param>
        /// <param name="cancellationToken">Token de cancelamento da operação</param>
        Task<TDocument> ObterPorId(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Listar os documentos da coleção
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento da operação</param>
        Task<IEnumerable<TDocument>> Listar(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adicionar um documento
        /// </summary>
        /// <param name="document">Instância do documento</param>
        /// <param name="cancellationToken">Token de cancelamento da operação</param>
        Task Adicionar(TDocument document, CancellationToken cancellationToken = default);

        /// <summary>
        /// Alterar um documento existente
        /// </summary>
        /// <param name="document">Instância do documento</param>
        /// <param name="cancellationToken">Token de cancelamento da operação</param>
        Task Editar(TDocument document, CancellationToken cancellationToken = default);

        /// <summary>
        /// Excluir um documento existente
        /// </summary>
        /// <param name="document">Instância do documento</param>
        /// <param name="cancellationToken">Token de cancelamento da operação</param>
        Task Remover(TDocument document, CancellationToken cancellationToken = default);

    }

}
