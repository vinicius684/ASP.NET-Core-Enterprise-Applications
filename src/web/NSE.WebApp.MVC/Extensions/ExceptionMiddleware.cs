using Grpc.Core;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using Polly.CircuitBreaker;
using Refit;
using System.Net;

namespace NSE.Identidade.API.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private static IAutenticacaoService _autenticacaoService;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //middleware padrão faço o que tenho que fazer e dou o next, aqui já dou o next e se tiver alguma exception eu trato
        //estratégio interessante inclusive para centralizar tratamento de exceptions. Posso criar outra exception customizada pra outros casos e tratar aqui dentro inclusive. Não preciso ficar espalhando try catch pela app inteira
        public async Task InvokeAsync(HttpContext httpContext, IAutenticacaoService autenticacaoService) 
        {
            _autenticacaoService = autenticacaoService;

            try
            {
                await _next(httpContext);
            }
            catch (CustomHttpRequestException ex)
            {
                HandleRequestExceptionAsync(httpContext, ex.StatusCode);
            }
            catch (ValidationApiException ex)//exception refit
            {
                HandleRequestExceptionAsync(httpContext, ex.StatusCode);
            }
            catch (ApiException ex) //exception refit
            {
                HandleRequestExceptionAsync(httpContext, ex.StatusCode);
            }
            catch (BrokenCircuitException)
            {
                HandleCircuitBreakerExceptionAsync(httpContext);
            }
            catch (RpcException ex)
            {
                //400 Bad Request	    INTERNAL
                //401 Unauthorized      UNAUTHENTICATED
                //403 Forbidden         PERMISSION_DENIED
                //404 Not Found         UNIMPLEMENTED

                var statusCode = ex.StatusCode switch
                {
                    StatusCode.Internal => 400,
                    StatusCode.Unauthenticated => 401,
                    StatusCode.PermissionDenied => 403,
                    StatusCode.Unimplemented => 404,
                    _ => 500
                };

                var httpStatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), statusCode.ToString());

                HandleRequestExceptionAsync(httpContext, httpStatusCode);
            }
        }

        private static void HandleRequestExceptionAsync(HttpContext context, HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.Unauthorized)//401 - usuário não conhecido
            {
                if (_autenticacaoService.TokenExpirado())
                {
                    if (_autenticacaoService.RefreshTokenValido().Result)//não posso chamar o RefreshTokenValido() com await, pois está dentro de um método void, por isso necessário pegar o .Result
                    {
                        context.Response.Redirect(context.Request.Path);//response redirect pra onde ele quer ir
                        return;
                    }
                }

                //se nada der certo, token expirado, mas não cosegui renovar, talvez o próprio refreshToken esteja expirado ou talvez não seja um problema referente a expiração
                _autenticacaoService.Logout();

                context.Response.Redirect($"/login?ReturnUrl={context.Request.Path}");//redireciona para o login e guarda a rota que vc estava antes, te possibilitando voltar para lá
                return;
            }

            context.Response.StatusCode = (int)statusCode;//caso não seja 401, vai ser o statusCode da minha exception
        }

        private static void HandleCircuitBreakerExceptionAsync(HttpContext context)
        {
            context.Response.Redirect("/sistema-indisponivel");
        }

    }
}
