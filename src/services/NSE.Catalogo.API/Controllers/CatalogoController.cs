using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Catalogo.API.Models;
using NSE.WebAPI.Core.Controllers;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Catalogo.API.Controllers
{
    /*
        IMPORTANTISSÍMO: Actions Retornando a própria entidade, mtas vezes isso não é uma boa prática dependendo da sua entidade. 
        Nesse caso como a entidade é simples, não tem problema retornar a entidade direto, agora quando a entidade tem mto mais 
        informações do que vc deseja mostrar, o ideal é que vc retorne um objeto response que represente sua entidade.
     */
    public class CatalogoController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;

        public CatalogoController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }


        [HttpGet("catalogo/produtos")]
        public async Task<PagedResult<Produto>> Index([FromQuery] int ps = 8, [FromQuery] int page = 1, [FromQuery] string q = null)
        {
            return await _produtoRepository.ObterTodos(ps, page, q);
        }

        //[ClaimsAuthorize("Catalogo", "Ler")]
        [HttpGet("catalogo/produtos/{id}")]
        public async Task<Produto> ProdutoDetalhe(Guid id)
        {
            return await _produtoRepository.ObterPorId(id);
        }

        [HttpGet("catalogo/produtos/lista/{ids}")]
        public async Task<IEnumerable<Produto>> ObterProdutosPorId(string ids)//string de ids, em bff CatalogoService, Join nos ids, separando por virgula. Em ObterProdutosPorId, dou um split pegando id por id
        {
            return await _produtoRepository.ObterProdutosPorId(ids);
        }
    }
}
