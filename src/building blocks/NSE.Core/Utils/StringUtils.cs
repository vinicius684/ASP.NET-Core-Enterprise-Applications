using System.Linq;

namespace NSE.Core.Utils
{
    public static class StringUtils
    {
        public static string ApenasNumeros(this string str, string input) //método de extensão da string - retorna apenas os números da string
        {
            return new string(input.Where(char.IsDigit).ToArray()); //char.IsDigit verifica se o caractere é um número
        }
    }
}