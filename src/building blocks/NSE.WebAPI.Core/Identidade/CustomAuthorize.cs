using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NSE.WebAPI.Core.Identidade
{
    public class CustomAuthorization //validar claims
    {
        public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue) //a partir do contexto da requisição, e com base no nome da claim e valor da claim
        {
            return context.User.Identity.IsAuthenticated && //vou verificar se o usuário está autenticado
                   context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));//depois vou verificar se ele possui o claimName e ClaimValue 
        }
    }

    public class ClaimsAuthorizeAttribute : TypeFilterAttribute //atributo que vai decorar um método com base no requisito
    {
        public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequisitoClaimFilter))//filtro que vai trabalhar diretamente com Authorizationfilter
        {
            Arguments = new object[] { new Claim(claimName, claimValue) };
        }
    }

    
    public class RequisitoClaimFilter : IAuthorizationFilter //requisito que implementa o filtro de autorização do próprio asp.net e faz a validação dos claims
    {
        private readonly Claim _claim;

        public RequisitoClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated) //se não está autenticado
            {
                context.Result = new StatusCodeResult(401);//não sei quem vc eh
                return;
            }

            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value)) //está autenticado, mas não tem a claim
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
