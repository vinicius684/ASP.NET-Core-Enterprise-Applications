using NSE.WebApp.MVC.Extensions;
using System.Net;

namespace NSE.Identidade.API.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //middleware padrão faço o que tenho que fazer e dou o next, aqui já dou o next e se tiver alguma exception eu trato
        //estratégio interessante inclusive para centralizar tratamento de exceptions. Posso criar outra exception customizada pra outros casos e tratar aqui dentro inclusive. Não preciso ficar espalhando try catch pela app inteira
        public async Task InvokeAsync(HttpContext httpContext) 
        {
            try
            {
                await _next(httpContext);
            }
            catch (CustomHttpRequestException ex)
            {
                HandleRequestExceptionAsync(httpContext, ex);
            }
        }

        private static void HandleRequestExceptionAsync(HttpContext context, CustomHttpRequestException httpRequestException)
        {
            if (httpRequestException.StatusCode == HttpStatusCode.Unauthorized) //401 - usuário não conhecido
            {
                context.Response.Redirect($"/login?ReturnUrl={context.Request.Path}"); //redireciona para o login e guarda a rota que vc estava antes, te possibilitando voltar para lá
                return;
            }

            context.Response.StatusCode = (int)httpRequestException.StatusCode; //caso não seja 401, vai ser o statusCode da minha exception
        }
    }
}
