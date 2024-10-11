using Microsoft.Extensions.Options;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Extensions;
using Refit;

namespace NSE.WebApp.MVC.Services
{
    public interface ICatalogoService
    {
        Task<IEnumerable<ProdutoViewModel>> ObterTodos();
        Task<ProdutoViewModel> ObterPorId(Guid id);
    }

    public interface ICatalogoServiceRefit //nesse caso não preciso da classe concreta
    {
        [Get("/catalogo/produtos/")]
        Task<IEnumerable<ProdutoViewModel>> ObterTodos();

        [Get("/catalogo/produtos/{id}")]
        Task<ProdutoViewModel> ObterPorId(Guid id);
    }

    public class CatalogoService : Service, ICatalogoService
    {
        private readonly HttpClient _httpCliente;

        public CatalogoService(HttpClient httpCliente, IOptions<AppSettings> settings)
        {
            httpCliente.BaseAddress = new Uri(settings.Value.CatalogoUrl);

            _httpCliente = httpCliente;
        }

        public async Task<ProdutoViewModel> ObterPorId(Guid id)
        {
            var response = await _httpCliente.GetAsync($"/catalogo/produtos/{id}");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<ProdutoViewModel>(response);
        }

        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            var response = await _httpCliente.GetAsync($"/catalogo/produtos/");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<IEnumerable<ProdutoViewModel>>(response);
        }
    }
}
