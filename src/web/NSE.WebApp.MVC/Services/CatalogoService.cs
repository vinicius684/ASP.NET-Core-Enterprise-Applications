using Microsoft.Extensions.Options;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Extensions;
using Refit;
using System.Net.Http;

namespace NSE.WebApp.MVC.Services
{
    public interface ICatalogoService
    {
        Task<PagedViewModel<ProdutoViewModel>> ObterTodos(int pageSize, int pageIndex, string query = null);
        Task<ProdutoViewModel> ObterPorId(Guid id);
    }

    //public interface ICatalogoServiceRefit //nesse caso não preciso da classe concreta
    //{
    //    [Get("/catalogo/produtos/")]
    //    Task<IEnumerable<ProdutoViewModel>> ObterTodos();

    //    [Get("/catalogo/produtos/{id}")]
    //    Task<ProdutoViewModel> ObterPorId(Guid id);
    //}

    public class CatalogoService : Service, ICatalogoService
    {
        private readonly HttpClient _httpClient;

        public CatalogoService(HttpClient httpCliente, IOptions<AppSettings> settings)
        {
            httpCliente.BaseAddress = new Uri(settings.Value.CatalogoUrl);

            _httpClient = httpCliente;
        }

        public async Task<ProdutoViewModel> ObterPorId(Guid id)
        {
            var response = await _httpClient.GetAsync($"/catalogo/produtos/{id}");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<ProdutoViewModel>(response);
        }

        public async Task<PagedViewModel<ProdutoViewModel>> ObterTodos(int pageSize, int pageIndex, string query = null)
        {
            var response = await _httpClient.GetAsync($"/catalogo/produtos?ps={pageSize}&page={pageIndex}&q={query}");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<PagedViewModel<ProdutoViewModel>>(response);
        }
    }
}
