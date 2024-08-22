using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSE.Identidade.API.Models;

namespace NSE.Identidade.API.Controllers
{
    [Route("api/identidade")]
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;//gerenciar login
        private readonly UserManager<IdentityUser> _userManager;//gerenciar usuário

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = new IdentityUser//a instancia não requer a senha, ela será passada a parte porque vai ser criptografada etc
            {
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true //se quiser habilitar para que o Usuário confirme o email depois e aí terá que enviar um email de confirmação, é config do identity. Olhar na Doc
            };

            var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            { 
                await _signInManager.SignInAsync(user, isPersistent: false);//login - isPersistent é se o usuário vai ser lembrado independente do do periodo de login definido
                return Ok();
            }

            return BadRequest();   
        }

        [HttpPost("autenticar")]
        public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if(!ModelState.IsValid) return BadRequest();

            var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true); //validar login conforme a senha

            if (result.Succeeded)
            { 
                return Ok();
            }

            return BadRequest();
        }
    }
}
