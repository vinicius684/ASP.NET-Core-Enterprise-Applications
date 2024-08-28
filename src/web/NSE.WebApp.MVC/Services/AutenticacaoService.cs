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

        public async Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin)
        {
            //o conteúdo a ser enviado deve ser serializado
            var loginContent = new StringContent( //criar o corpo de uma requisição HTTP quando o conteúdo é uma string.
                JsonSerializer.Serialize(usuarioLogin),
                 Encoding.UTF8,
                "application/json"//colocado no header da requisição
            );

            var response = await _httpCliente.PostAsync("https://localhost:44375/api/identidade/autenticar", loginContent);

            var options = new JsonSerializerOptions //Não vai ligar para maiusculo e minusco(nome atributos) para deserializar o Json recebido para meu objeto UsuarioRepostaLogin
            {
                PropertyNameCaseInsensitive = true,
            };

            return JsonSerializer.Deserialize<UsuarioRespostaLogin>(await response.Content.ReadAsStringAsync(), options);
        }

        public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro)
        {
            var registroContent = new StringContent(
                JsonSerializer.Serialize(usuarioRegistro),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpCliente.PostAsync("https://localhost:44375/api/identidade/nova-conta", registroContent);

            return JsonSerializer.Deserialize<UsuarioRespostaLogin>(await response.Content.ReadAsStringAsync());
        }
    }
}
