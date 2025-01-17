using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Services;

namespace NSE.WebApp.MVC.Controllers
{
    public class CatalogoController : MainController
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogoController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet]
        [Route("")]
        [Route("vitrine")]
        public async Task<IActionResult> Index([FromQuery] int ps = 8, [FromQuery] int page = 1, [FromQuery] string q = null)
        {
            var produtos = await _catalogoService.ObterTodos(ps, page, q);
            
            ViewBag.Pesquisa = q;//caixa de texto na layout define valor da query, navegação paginação na Catalogo Index.cshtml pega o valor da Model.Query e envia pra Action, Action define o valor da ViewBag.Pesquisa = q, a caixa de texto da pesquisa salva o texto pesquisado ao navegar pela paginação
            produtos.ReferenceAction = "Index";//paso para tornar o componente de nevegação da paginação reutilizável, ao utilizar paginação em outra controller ou action tenho que definir a ReferenceAction aqui

            return View(produtos);
        }

        [HttpGet]
        [Route("produto-detalhe/{id}")]
        public async Task<IActionResult> ProdutoDetalhe(Guid id)
        {
            var produto = await _catalogoService.ObterPorId(id);

            return View(produto);
        }
    }
}