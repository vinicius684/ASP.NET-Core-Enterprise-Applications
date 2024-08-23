using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Extensions;
using NSE.Identidade.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NSE.Identidade.API.Controllers
{
    [ApiController] //dizendo que é uma API controller, libera o entendimento dos schemas do swagger, com isso trafegar json e não formulário
    [Route("api/identidade")]
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;//gerenciar login
        private readonly UserManager<IdentityUser> _userManager;//gerenciar usuário
        private readonly AppSettings _appSettings;

        public AuthController(SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings)//IOptions - opção de leitura que o prório asp.net te dá como suporte para ler aquivs de configuração
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
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
                return Ok(await GerarJwt(usuarioRegistro.Email));
            }

            return BadRequest();
        }

        [HttpPost("autenticar")]
        public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true); //validar login conforme a senha

            if (result.Succeeded)
            {
                return Ok(await GerarJwt(usuarioLogin.Email));
            }

            return BadRequest();
        }


        private async Task<UsuarioRespostaLogin> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            //Com base na coleção de claims obtida, vou adicionar essas 5 claims específicas para JWT. Infos douser passadas no token em formato de claims
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            //objeto real da coleção de claims que aql usuário vai ter na representação dele no JWT - objeto esperado pelo Subject para representar as Claims do JWT 
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();//vai pegar com base na chave que eu tenho e gerar meu token, utilizando infos da Instanciação de SecurityTokenDescriptor
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,//Dados do user
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),//Duas hrs pra frente, no padrão UTC
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);//Tranforma o objeto SecurityToken gerado pelo CreateToken em uma string codificada em formato JWT que pode ser facilmente transportada

            var response = new UsuarioRespostaLogin
            {
                AccessToken = encodedToken,//meu token codificado
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                }
            };

            return response;
        }

        private static long ToUnixEpochDate(DateTime date) //pegar um data e transformar ela no padrão que o JWT espera, EpochDate
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    }
}
