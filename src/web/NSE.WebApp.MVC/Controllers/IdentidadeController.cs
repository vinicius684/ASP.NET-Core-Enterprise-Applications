using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace NSE.WebApp.MVC.Controllers
{
    public class IdentidadeController : Controller
    {
        private readonly IAutenticacaoService _autenticacaoService;

        public IdentidadeController(IAutenticacaoService autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }

        [HttpGet]
        [Route("nova-conta")]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [Route("nova-conta")]
        public async Task<IActionResult> Registro(UsuarioRegistro usuarioRegistro) //receber post para fazer o registro chamando a api
        {
            if (!ModelState.IsValid) return View(usuarioRegistro);

            //API - Registro
            var resposta = await _autenticacaoService.Registro(usuarioRegistro); 

            //if (ResponsePossuiErros(resposta.ResponseResult)) return View(usuarioRegistro);

            await RealizarLogin(resposta);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string returnUrl = null)
        {
            //ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin, string returnUrl = null)
        {
            //ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(usuarioLogin);

            //API - Login
            var resposta = await _autenticacaoService.Login(usuarioLogin); //retorna Obj com jwt, claims e demais infos

            //if (ResponsePossuiErros(resposta.ResponseResult)) return View(usuarioLogin);

            //Login na app
            await RealizarLogin(resposta);

            //if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Home");

            //return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("sair")]
        public async Task<IActionResult> Logout() //Limpar o cookie de autenticação para que o usuário não seja mais entendido como um usário logado
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task RealizarLogin(UsuarioRespostaLogin resposta)
        {
            var token = ObterTokenFormatado(resposta.AccessToken);

            var claims = new List<Claim>();
            claims.Add(new Claim("JWT", resposta.AccessToken));
            claims.AddRange(token.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); //quer dizer que vai cosneguir gerar as claims dentro do cookie

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                IsPersistent = true //durar multiplos requests dentro do periodoo acima
            };

            //processo de login - através do próprio asp.net core, ou seja, autenticar o usuário e criar uma sessão autenticada
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
             );
        }

        private static JwtSecurityToken ObterTokenFormatado(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
        }
    }
}
