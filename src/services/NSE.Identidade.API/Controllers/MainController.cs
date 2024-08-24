using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSE.Identidade.API.Controllers
{
    [ApiController] //dizendo que é uma API controller, libera o entendimento dos schemas do swagger, com isso trafegar json e não formulário
    public abstract class MainController : Controller //abstract, só pode ser herdada
    {
        protected ICollection<string> Erros = new List<string>();

        protected ActionResult CustomResponse(object result = null) // =null é pra não ser obrigado a informar
        {
            if (OperacaoValida())
            {
                return Ok(result);
            }

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]> //ValidationProblemDetails - implementa um padrão especificada numa RFC, que diz como um api deve responder sobre detalhes de erros
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState) //Sobrecarga, se for um erro de validação da VM. Onde vou receber o resultado da validação da VM
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }

            return CustomResponse();
        }

        protected bool OperacaoValida()
        {
            return !Erros.Any();
        }

        protected void AdicionarErroProcessamento(string erro)
        {
            Erros.Add(erro);
        }

        protected void LimparErrosProcessamento()
        {
            Erros.Clear();
        }
    }
}
