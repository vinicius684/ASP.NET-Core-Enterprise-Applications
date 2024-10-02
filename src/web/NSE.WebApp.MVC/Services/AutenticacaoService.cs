using NSE.WebApp.MVC.Models;
using System.Runtime.Intrinsics.X86;
using System;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using NSE.WebApp.MVC.Extensions;
using Microsoft.Extensions.Options;

namespace NSE.WebApp.MVC.Services
{
    public class AutenticacaoService : Service, IAutenticacaoService
    {
        private readonly HttpClient _httpCliente;
       // private readonly AppSettings _settings;

        public AutenticacaoService(HttpClient httpCliente, IOptions<AppSettings> settings)
        {
            httpCliente.BaseAddress = new Uri(settings.Value.AutenticacaoUrl);

            _httpCliente = httpCliente;
            //_settings = settings.Value;
        }

        public async Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin)
        {
            var loginContent = ObterConteudo(usuarioLogin);

            var response = await _httpCliente.PostAsync("/api/identidade/autenticar", loginContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    //ResponseResult = JsonSerializer.Deserialize<ResponseResult>(await response.Content.ReadAsStringAsync(), options)
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            //return JsonSerializer.Deserialize<UsuarioRespostaLogin>(await response.Content.ReadAsStringAsync(), options);
            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }

        public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro)
        {
            var registroContent = ObterConteudo(usuarioRegistro);

            var response = await _httpCliente.PostAsync("/api/identidade/nova-conta", registroContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin 
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }
    }
}
