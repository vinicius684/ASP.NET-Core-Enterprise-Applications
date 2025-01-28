using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSE.Identidade.API.Extensions;
using NSE.Identidade.API.Models;
using NSE.WebAPI.Core.Identidade;
using NSE.WebAPI.Core.Controllers;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using NSE.WebAPI.Core.Usuario;
using NetDevPack.Security.Jwt.Core.Interfaces;
using NSE.Identidade.API.Services;

namespace NSE.Identidade.API.Controllers
{
    [Route("api/identidade")]
    public class AuthController : MainController
    {
        private readonly AuthenticationService _authenticationService;
        private readonly IMessageBus _bus;

        public AuthController(
                              AuthenticationService authenticationService, 
                              IMessageBus bus)
        {
           _authenticationService = authenticationService;
            _bus = bus;
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

            var result = await _authenticationService._userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            {
                //Integração
                var clienteReuslt = await RegistrarCliente(usuarioRegistro);

                if (!clienteReuslt.ValidationResult.IsValid) //tratamento regrade negócio, sucesso ou não da operação de criar clien
                {
                    await _authenticationService._userManager.DeleteAsync(user);
                    return CustomResponse(clienteReuslt.ValidationResult);
                    throw new Exception("Erro ao validar o cliente");
                }

                return CustomResponse(await _authenticationService.GerarJwt(usuarioRegistro.Email));
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

            var result = await _authenticationService._signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true); //validar login conforme a senha

            if (result.Succeeded)
            {
                return CustomResponse(await _authenticationService.GerarJwt(usuarioLogin.Email));
            }

            if (result.IsLockedOut)
            {
                AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse();
            }

            AdicionarErroProcessamento("Usuário ou Senha incorretos");
            return CustomResponse();
        }

        private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistro usuarioRegistro)
        {
            var usuario = await _authenticationService._userManager.FindByEmailAsync(usuarioRegistro.Email); //todas as infos do usuário principalmente o Id que o UsuarioRegistro não entrega

            var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(
                Guid.Parse(usuario.Id), usuarioRegistro.Nome, usuarioRegistro.Email, usuarioRegistro.Cpf);

            try//tratamento de exception, erro no bus. Nesse caso vai "jogar a exception pra cima", a mvc vai pegar
            {
                return await _bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
            }
            catch
            {
                await _authenticationService._userManager.DeleteAsync(usuario);
                throw;
            }
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] string refreshToken)//receber através do corpo da requisição um string refreshToken
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                AdicionarErroProcessamento("Refresh Token inválido");
                return CustomResponse();
            }

            var token = await _authenticationService.ObterRefreshToken(Guid.Parse(refreshToken));//aqui retorna um tokn preenchido ou nulo se nõ for mais válido

            if (token is null)
            {
                AdicionarErroProcessamento("Refresh Token expirado");
                return CustomResponse();
            }

            return CustomResponse(await _authenticationService.GerarJwt(token.Username));
        }

    }
}

