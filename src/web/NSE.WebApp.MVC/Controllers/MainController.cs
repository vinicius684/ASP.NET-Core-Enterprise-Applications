using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Controllers
{
    public class MainController: Controller 
    {
        protected bool ResponsePossuiErros(ResponseResult resposta)
        {
            if (resposta != null && resposta.Errors.Mensagens.Any())
            {
                foreach (var mensagem in resposta.Errors.Mensagens)
                {
                    ModelState.AddModelError(string.Empty, mensagem); //pegando cada mensagem e adcionando-a como um erro dentro da modelState de ResponseResult (Erro de Model). Assim é possível repassar os erros para o usuário, inclusive motrar na view
                }

                return true;
            }

            return false;
        }
    }
}
