using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSE.Bff.Compras.Services
{
    public abstract class Service
    {
        protected StringContent ObterConteudo(object dado)
        {
            return new StringContent(
                JsonSerializer.Serialize(dado),
                Encoding.UTF8,
                "application/json");
        }

        protected async Task<T> DeserializarObjetoResponse<T>(HttpResponseMessage responseMessage)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        }

        protected bool TratarErrosResponse(HttpResponseMessage response)//Mais simples do que no MVC
        {
            if (response.StatusCode == HttpStatusCode.BadRequest) return false;//se for 400, retorno falso

            response.EnsureSuccessStatusCode();//se for código de 200 retorna true
            return true;

            //qualquer outro erro vou deixar "explodir", vai cair lá embaixo aonde vai ser tratado
        }
    }
}