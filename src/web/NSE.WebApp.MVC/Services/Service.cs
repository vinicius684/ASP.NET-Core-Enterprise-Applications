using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NSE.WebApp.MVC.Extensions;

namespace NSE.WebApp.MVC.Services
{
    public abstract class Service
    {
        //protected StringContent ObterConteudo(object dado)
        //{
        //    return new StringContent(
        //        JsonSerializer.Serialize(dado),
        //        Encoding.UTF8,
        //        "application/json");
        //}

        //protected async Task<T> DeserializarObjetoResponse<T>(HttpResponseMessage responseMessage)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //    return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        //}

        protected bool TratarErrosResponse(HttpResponseMessage response)
        {
            switch ((int)response.StatusCode)
            {
                case 401:
                case 403:
                case 404:
                case 500:
                    throw new CustomHttpRequestException(response.StatusCode);

                case 400:
                    return false; //caso em que tenho mensagens de erro dentro do meu response e quero pega-las
            }

            response.EnsureSuccessStatusCode();//garanta que retornou um dos códigos de sucesso, caso não, estoura uma exception
            return true;
        }
    }
}