using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Bff.Compras.Services;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Bff.Compras.Controllers
{
    [Authorize]
    public class CarrinhoController : MainController
    {
        private readonly ICarrinhoService _carrinhoService;
        private readonly ICatalogoService _catalogoService;

        public CarrinhoController(
            ICarrinhoService carrinhoService, 
            ICatalogoService catalogoService)
        { 
            _carrinhoService = carrinhoService;
            _catalogoService = catalogoService; 
        }

        [HttpGet]
        [Route("compras/carrinho")]
        public async Task<IActionResult> Index()
        {
            return CustomResponse();
        }

        [HttpGet]
        [Route("compras/carrinho-quantidade")]
        public async Task<IActionResult> ObterQuantidadeCarrinho()
        {   /*
               Novidade, obter apenas  qtd de itens do carrinho, pra não ter que pegar o carinho inteiro pra obter a quantidade

               Nota: ObterQuantidadeCarrinho é mais  pra atender um necessidade de Front-end.E nossa API de carrinho não está orientada a 
               atender o front, mas sim a trabalhar com o Carrrinho. Como temos um BFF é ele que deve se preocupar em atendeer o front e 
               te dar o dado mais preparado o possível

               Pode ser que queira resolver isso na API Carrinho, mas a mensagem principal é, se tem um BFF, qualquer necessidade pra atender o front deve ser resolvida aqui.
             */


            return CustomResponse();
        }

        [HttpPost]
        [Route("compras/carrinho/items")]
        public async Task<IActionResult> AdicionarItemCarrinho()
        {
            return CustomResponse();
        }

        [HttpPut]
        [Route("compras/carrinho/items/{produtoId}")]
        public async Task<IActionResult> AtualizarItemCarrinho()
        {
            return CustomResponse();
        }

        [HttpDelete]
        [Route("compras/carrinho/items/{produtoId}")]
        public async Task<IActionResult> RemoverItemCarrinho()
        {
            return CustomResponse();
        }
    }
}
