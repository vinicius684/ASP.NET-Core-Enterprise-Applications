using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;


namespace NSE.WebApp.MVC.Controllers
{
    public class IdentidadeController : MainController
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

            if (ResponsePossuiErros(resposta.ResponseResult)) return View(usuarioRegistro);

            //APP Login
            await _autenticacaoService.RealizarLogin(resposta);

            return RedirectToAction("Index", "Catalogo");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string returnUrl = null) //vai receber um parâmero de rota que representa a rota que ele estava ates do redirecionamento para login do 401
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(usuarioLogin);

            //API - Login
            var resposta = await _autenticacaoService.Login(usuarioLogin); //retorna Obj com jwt, claims e demais infos

            if (ResponsePossuiErros(resposta.ResponseResult)) return View(usuarioLogin);

            //Login na app
            await _autenticacaoService.RealizarLogin(resposta);

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Catalogo");

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        [Route("sair")]
        public async Task<IActionResult> Logout() //Limpar o cookie de autenticação para que o usuário não seja mais entendido como um usário logado
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _autenticacaoService.Logout();
            return RedirectToAction("Index", "Catalogo");
        }
       
       
    }
}
