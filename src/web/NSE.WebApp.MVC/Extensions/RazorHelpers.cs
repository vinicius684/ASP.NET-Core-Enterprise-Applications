using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Cryptography;
using System.Text;

namespace NSE.WebApp.MVC.Extensions
{
    public static class RazorHelpers
    {
        //forma de conseguir a imagem de um gravatar específico
        public static string HashEmailForGravatar(this RazorPage page, string email)//quando meu usuário fizer login, se ele tiver uma conta nog ravatar(aplicação web que vc registra um avatar pra um email), vou querer pegar a foto do gravatar dele
        {
            var md5Hasher = MD5.Create(); //criar um hash md5
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));//computar esse hash com base nos bytes do email
            var sBuilder = new StringBuilder();
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));//através do stringBuilder vou fazer o build dos bytes do email com a string x2
            }
            return sBuilder.ToString();//tranformar o builder numa string
        }

        public static string FormatoMoeda(this RazorPage page, decimal valor)
        {
            return FormatoMoeda(valor);
        }

        private static string FormatoMoeda(decimal valor)
        {
            return string.Format(Thread.CurrentThread.CurrentCulture, "{0:C}", valor);
        }

        public static string MensagemEstoque(this RazorPage page, int quantidade) //extendendo o próprio Razor Page, criando método dentro do razor Page
        {
            return quantidade > 0 ? $"Apenas {quantidade} em estoque!" : "Produto esgotado!";
        }

        public static string UnidadesPorProduto(this RazorPage page, int unidades)
        {
            return unidades > 1 ? $"{unidades} unidades" : $"{unidades} unidade";
        }

        public static string UnidadesPorProdutoValorTotal(this RazorPage page, int unidades, decimal valor)
        {
            return $"{unidades}x {FormatoMoeda(valor)} = Total: {FormatoMoeda(valor * unidades)}";
        }

        public static string SelectOptionsPorQuantidade(this RazorPage page, int quantidade, int valorAtual = 0) //Extensão basicamente para deixar o DropDown selecionado na QtdAtual
        {
            var sb = new StringBuilder();
            for (var i = 1; i <= quantidade; i++)
            {
                var selected = "";
                if (i == valorAtual) selected = "selected"; //se o valorAtual for igual ao i ele ganha um selected. senão vai ficar uma option não selecionada.
                sb.Append($"<option {selected} value='{i}'>{i}</option>");
            }

            return sb.ToString();
        }
        public static string ExibeStatus(this RazorPage page, int status)
        {
            var statusMensagem = "";
            var statusClasse = "";

            switch (status)
            {
                case 1:
                    statusClasse = "info";
                    statusMensagem = "Em aprovação";
                    break;
                case 2:
                    statusClasse = "primary";
                    statusMensagem = "Aprovado";
                    break;
                case 3:
                    statusClasse = "danger";
                    statusMensagem = "Recusado";
                    break;
                case 4:
                    statusClasse = "success";
                    statusMensagem = "Entregue";
                    break;
                case 5:
                    statusClasse = "warning";
                    statusMensagem = "Cancelado";
                    break;

            }

            return $"<span class='badge badge-{statusClasse}'>{statusMensagem}</span>";
        }
    }
}
