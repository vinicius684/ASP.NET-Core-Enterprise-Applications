using NSE.WebApp.MVC.Models;
using System.Runtime.Intrinsics.X86;
using System;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace NSE.WebApp.MVC.Services
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly HttpClient _httpCliente;

        public AutenticacaoService(HttpClient httpCliente)
        {
            _httpCliente = httpCliente;
        }

        public async Task<string> Login(UsuarioLogin usuarioLogin)
        {
            //o conteúdo a ser enviado deve ser serializado
            var loginContent = new StringContent( //criar o corpo de uma requisição HTTP quando o conteúdo é uma string.
                JsonSerializer.Serialize(usuarioLogin),
                 Encoding.UTF8,
                "application/json"//colocado no header da requisição
            );

            var response = await _httpCliente.PostAsync("https://localhost:44375/api/identidade/autenticar", loginContent);

            return JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync());
        }

        public async Task<string> Registro(UsuarioRegistro usuarioRegistro)
        {
            var registroContent = new StringContent(
                JsonSerializer.Serialize(usuarioRegistro),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpCliente.PostAsync("https://localhost:44375/api/identidade/nova-conta", registroContent);

            return JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync());
        }
    }
}
