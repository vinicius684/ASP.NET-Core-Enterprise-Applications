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
    [Route("api/identidade")]
    public class AuthController : MainController
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
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser//a instancia não requer a senha, ela será passada a parte porque vai ser criptografada etc
            {
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true //se quiser habilitar para que o Usuário confirme o email depois e aí terá que enviar um email de confirmação, é config do identity. Olhar na Doc
            };

            var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            {
               //await _signInManager.SignInAsync(user, isPersistent: false);//se o registro deu sucesso, não há necessidade de fazer o login a n ser que esteja trabalhando diretamente na aplicação que vai interpretar o usuário. Aqui só quero gerar o token pra alguem utilizar
                return CustomResponse(await GerarJwt(usuarioRegistro.Email));
            }

            foreach (var error in result.Errors)
            {
                AdicionarErroProcessamento(error.Description);
            }

            return CustomResponse();
        }

        [HttpPost("autenticar")]
        public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true); //validar login conforme a senha

            if (result.Succeeded)
            {
                return CustomResponse(await GerarJwt(usuarioLogin.Email));
            }

            if (result.IsLockedOut)
            {
                AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse();
            }

            AdicionarErroProcessamento("Usuário ou Senha incorretos");
            return CustomResponse();
        }

        private async Task<UsuarioRespostaLogin> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email); //obtenho user do identity
            var claims = await _userManager.GetClaimsAsync(user); //suas claims Identity

            var identityClaims = await ObterClaimsUsuario(claims, user); //passo minha lista de claim do idenentity do Usuário, que será somada às Clains específicas do JWT + roles do identity. Aqui é tipo refeência que é passado(List), portanto mesmo estando em escopo diferente, a var claims passa a receber todas as claims incrementadas nesse método
            var encodedToken = CodificarToken(identityClaims); //Crio e codifico o Token

            return ObterRespostaToken(encodedToken, user, claims); //Resposta final, representação do Usuário na minha app, contendo o Token
        }

        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user) //Populo Claims específica JWT e Roles Identity e devolto o Objeto ClaimIdentity esperado pelo user
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

        private string CodificarToken(ClaimsIdentity identityClaims) //Criação e  códigicação do JWT
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims) //Resposta representação do meu Usuário na app, contendo o token
        {
            return new UsuarioRespostaLogin
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        private static long ToUnixEpochDate(DateTime date) //pegar um data e transformar ela no padrão que o JWT espera, EpochDate
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    }
}

//Geração do JWT em método unico antes da refatoração
//private async Task<UsuarioRespostaLogin> GerarJwt(string email)
//{
//    var user = await _userManager.FindByEmailAsync(email);
//    var claims = await _userManager.GetClaimsAsync(user);
//    var userRoles = await _userManager.GetRolesAsync(user);

//    //Com base na coleção de claims obtida, vou adicionar essas 5 claims específicas para JWT. Infos douser passadas no token em formato de claims
//    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
//    claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
//    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
//    claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
//    claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

//    foreach (var userRole in userRoles)
//    {
//        claims.Add(new Claim("role", userRole));
//    }

//    //objeto real da coleção de claims que aql usuário vai ter na representação dele no JWT - objeto esperado pelo Subject para representar as Claims do JWT 
//    var identityClaims = new ClaimsIdentity();
//    identityClaims.AddClaims(claims);

//    var tokenHandler = new JwtSecurityTokenHandler();//vai pegar com base na chave que eu tenho e gerar meu token, utilizando infos da Instanciação de SecurityTokenDescriptor
//    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
//    var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
//    {
//        Issuer = _appSettings.Emissor,
//        Audience = _appSettings.ValidoEm,
//        Subject = identityClaims,//Dados do user
//        Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),//Duas hrs pra frente, no padrão UTC
//        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//    });

//    var encodedToken = tokenHandler.WriteToken(token);//Tranforma o objeto SecurityToken gerado pelo CreateToken em uma string codificada em formato JWT que pode ser facilmente transportada

//    var response = new UsuarioRespostaLogin
//    {
//        AccessToken = encodedToken,//meu token codificado
//        ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
//        UsuarioToken = new UsuarioToken
//        {
//            Id = user.Id,
//            Email = user.Email,
//            Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
//        }
//    };

//    return response;
//}