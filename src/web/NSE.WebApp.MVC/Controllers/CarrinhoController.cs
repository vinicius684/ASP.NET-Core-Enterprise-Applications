using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;

namespace NSE.WebApp.MVC.Controllers
{
    public class CarrinhoController : MainController
    {
        //private readonly ICarrinhoService _carrinhoService;
        //private readonly ICatalogoService _catalogoService;

        //public CarrinhoController(ICarrinhoService carrinhoService,
        //                          ICatalogoService catalogoService)
        //{
        //    _carrinhoService = carrinhoService;
        //    _catalogoService = catalogoService;
        //}

        [Route("carrinho")]
        public async Task<IActionResult> Index()
        {
            //return View(await _carrinhoService.ObterCarrinho());
            return View();
        }

        [HttpPost]
        [Route("carrinho/adicionar-item")]
        public async Task<IActionResult> AdicionarItemCarrinho(ItemProdutoViewModel itemProduto)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("carrinho/atualizar-item")]
        public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, int quantidade)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("carrinho/remover-item")]
        public async Task<IActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            return RedirectToAction("Index");
        }

        //private void ValidarItemCarrinho(ProdutoViewModel produto, int quantidade)
        //{
        //    if (produto == null) AdicionarErroValidacao("Produto inexistente!");
        //    if (quantidade < 1) AdicionarErroValidacao($"Escolha ao menos uma unidade do produto {produto.Nome}");
        //    if (quantidade > produto.QuantidadeEstoque) AdicionarErroValidacao($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} unidades em estoque, você selecionou {quantidade}");
        //}
    }
}
