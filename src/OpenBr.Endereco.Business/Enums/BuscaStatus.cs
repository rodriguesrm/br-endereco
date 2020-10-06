using System;
using System.ComponentModel;

namespace OpenBr.Endereco.Business.Enums
{

    /// <summary>
    /// Enumeração de status de busca
    /// </summary>
    [Flags]
    public enum BuscaStatus
    {
        
        [Description("Pendente")]
        Pendente = 1,

        [Description("Sucesso na busca")]
        Sucesso = 2,

        [Description("Busca expirada")]
        Expirada = 3,

        [Description("Busca cancelada")]
        Cancelada = 4

    }
}
