using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NSE.WebApp.MVC.Extensions;

namespace NSE.WebApp.MVC.Services.Handlers
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler//A ideia é que quando crio um Delegatehandler, estou sobrescrevendo o SendAsync do meu http client. No escopo desse método posso fazer o que quiser com meu Request
    {
        private readonly IUser _user;

        public HttpClientAuthorizationDelegatingHandler(IUser user)
        {
            _user = user;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _user.ObterHttpContext().Request.Headers["Authorization"]; //através do meu contexto, verifico se o Header "Authorization" existe na requisição atual(Client)

            if (!string.IsNullOrEmpty(authorizationHeader)) //Se existe, copio esse cabeçalho para uma nova requisição HTTP que está sendo contruída(API)
            {
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }

            var token = _user.ObterUserToken();//garantindo de passar o token do usuário

            if (token != null)//adicionando-o à nova requisição como authenticationHeaderValue
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}