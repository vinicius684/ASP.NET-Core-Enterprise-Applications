using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;

namespace NSE.WebApp.MVC.Controllers
{
    public class CarrinhoController : MainController
    {
        private readonly IComprasBffService _comprasBffService;
       

        public CarrinhoController(IComprasBffService comprasBffService)
        {
            _comprasBffService = comprasBffService;
        }

        [Route("carrinho")]
        public async Task<IActionResult> Index()
        {
            return View(await _comprasBffService.ObterCarrinho());
        }

        [HttpPost]
        [Route("carrinho/adicionar-item")]
        public async Task<IActionResult> AdicionarItemCarrinho(ItemCarrinhoViewModel itemCarrinho)//Vou receber o ProdutoId pela ProdutoDetalhe.cshml em um input Hiden, não poasso receber todas as outras infos dessa forma, pois pdoeriam ser facilmente manipuladas no html, protanto vou obter as outras infos no CatalogoService por meio do produtoId recebido
        {
            var resposta = await _comprasBffService.AdicionarItemCarrinho(itemCarrinho);

            if (ResponsePossuiErros(resposta)) return View("Index", await _comprasBffService.ObterCarrinho()); //Retornanda view já com os erros que vão ser exibidos na tela.  Retorna a mesma página que o usuário estava, com os dados do modelo que você quer exibir (incluindo erros).

            return RedirectToAction("Index");//obtem itens do carrinho atualizado e Todos os dados do formulário anterior, como erros ou entradas do usuário, são perdidos. Útil para evitar reenvios
        }

        [HttpPost]
        [Route("carrinho/atualizar-item")]
        public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, int quantidade)
        {

            var item = new ItemCarrinhoViewModel { ProdutoId = produtoId, Quantidade = quantidade };
            var resposta = await _comprasBffService.AtualizarItemCarrinho(produtoId, item);

            if (ResponsePossuiErros(resposta)) return View("Index", await _comprasBffService.ObterCarrinho());

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("carrinho/remover-item")]
        public async Task<IActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            var resposta = await _comprasBffService.RemoverItemCarrinho(produtoId);

            if (ResponsePossuiErros(resposta)) return View("Index", await _comprasBffService.ObterCarrinho());

            return RedirectToAction("Index");
        }

        private void ValidarItemCarrinho(ProdutoViewModel produto, int quantidade)
        {
            if (produto == null) AdicionarErroValidacao("Produto inexistente!");
            if (quantidade < 1) AdicionarErroValidacao($"Escolha ao menos uma unidade do produto {produto.Nome}");
            if (quantidade > produto.QuantidadeEstoque) AdicionarErroValidacao($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} unidades em estoque, você selecionou {quantidade}");
        }
    }
}
