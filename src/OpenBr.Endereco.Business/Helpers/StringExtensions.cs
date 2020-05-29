using System;

namespace OpenBr.Endereco.Business.Helpers
{

    /// <summary>
    /// Extensões de string
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Converte a string para formato camelCase
        /// </summary>
        /// <param name="expression">Expressão texto a ser convertido</param>
        public static string ToCamelCase(this string expression)
            =>  Char.ToLowerInvariant(expression[0]) + expression.Substring(1);

    }
}
