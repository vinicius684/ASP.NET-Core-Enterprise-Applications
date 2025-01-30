using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Extensions;
using NSE.Identidade.API.Models;
using NSE.WebAPI.Core.Identidade;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NSE.WebAPI.Core.Usuario;
using NetDevPack.Security.Jwt.Core.Interfaces;
using NSE.Identidade.API.Data;
using Microsoft.EntityFrameworkCore;

namespace NSE.Identidade.API.Services
{
    public class AuthenticationService
    {
        public readonly SignInManager<IdentityUser> _signInManager;//gerenciar login
        public readonly UserManager<IdentityUser> _userManager;//gerenciar usuário
        private readonly AppSettings _appSettings;
        private readonly AppTokenSettings _appTokenSettingsSettings;
        private readonly ApplicationDbContext _context;

        private readonly IJwtService _jwtService;
        private readonly IAspNetUser _aspNetUser;

        public AuthenticationService(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings,//IOptions - opção de leitura que o prório asp.net te dá como suporte para ler aquivs de configuração
            IOptions<AppTokenSettings> appTokenSettingsSettings,
            ApplicationDbContext context,
            IJwtService jwksService,
            IAspNetUser aspNetUser)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _appTokenSettingsSettings = appTokenSettingsSettings.Value;
            _jwtService = jwksService;
            _aspNetUser = aspNetUser;
            _context = context;
        }

        public async Task<UsuarioRespostaLogin> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email); //obtenho user do identity
            var claims = await _userManager.GetClaimsAsync(user); //suas claims Identity

            var identityClaims = await ObterClaimsUsuario(claims, user); //passo minha lista de claim do idenentity do Usuário, que será somada às Clains específicas do JWT + roles do identity. Aqui é tipo refeência que é passado(List), portanto mesmo estando em escopo diferente, a var claims passa a receber todas as claims incrementadas nesse método
            var encodedToken = await CodificarToken(identityClaims); //Crio e codifico o Token

            var refreshToken = await GerarRefreshToken(email);

            return ObterRespostaToken(encodedToken, user, claims, refreshToken); //Resposta final, representação do Usuário na minha app, contendo o Token
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

        private async Task<string> CodificarToken(ClaimsIdentity identityClaims) //Criação e  códigicação do JWT
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var currentIssuer =
                $"{_aspNetUser.ObterHttpContext().Request.Scheme}://{_aspNetUser.ObterHttpContext().Request.Host}"; //próprio endpoint da api de autenticação

            var key = await _jwtService.GetCurrentSigningCredentials();

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddMinutes(1),//quando o token vai expirar
                SigningCredentials = key
            });

            return tokenHandler.WriteToken(token);
        }

        private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims, RefreshToken refreshToken) //Resposta representação do meu Usuário na app, contendo o token
        {
            return new UsuarioRespostaLogin
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,//informando no token quanto de tempo ele está válido
                RefreshToken = refreshToken.Token,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);

        private async Task<RefreshToken> GerarRefreshToken(string email)
        {
            var refreshToken = new RefreshToken
            {
                Username = email,
                ExpirationDate = DateTime.UtcNow.AddHours(_appTokenSettingsSettings.RefreshTokenExpiration)//Se estiver trabalhando coms ervidores em diversas localidade, importante colocar UtcNow. Daí vou poder comparar esse horário com meu horário local Convertendo
            };

            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(u => u.Username == email));//reemover um series de refresh tokens, só vai ter um, mas por segurança caso haja mais...
            await _context.RefreshTokens.AddAsync(refreshToken);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken> ObterRefreshToken(Guid refreshToken)
        {
            var token = await _context.RefreshTokens.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Token == refreshToken);

            return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now //se o token obtido for diferente de nulo e ainda não tiver expirado, vou retornar o RefreshToken
                ? token
                : null;
        }
    }
}