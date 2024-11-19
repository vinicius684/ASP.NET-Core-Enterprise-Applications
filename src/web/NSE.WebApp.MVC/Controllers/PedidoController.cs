using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;

namespace NSE.WebApp.MVC.Controllers
{
    public class PedidoController : MainController
    {
        private readonly IClienteService _clienteService;
        private readonly IComprasBffService _comprasBffService;

        public PedidoController(IClienteService clienteService,
            IComprasBffService comprasBffService)
        {
            _clienteService = clienteService;
            _comprasBffService = comprasBffService;
        }

        [HttpGet]
        [Route("endereco-de-entrega")]
        public async Task<IActionResult> EnderecoEntrega()// Primeiro paço do pedido, setar o endereço de entrega
        {
            var carrinho = await _comprasBffService.ObterCarrinho();
            if (carrinho.Itens.Count == 0) return RedirectToAction("Index", "Carrinho");//se o carrinho não tiver nada, redireciono ele para um carrinho novo

            var endereco = await _clienteService.ObterEndereco(); //pra saber se vou pedir endereço ou não. Faz mais sentido dentro da _Endereco.cshtml
            var pedido = _comprasBffService.MapearParaPedido(carrinho, endereco);

            return View(pedido);
        }

        [HttpGet]
        [Route("pagamento")]
        public async Task<IActionResult> Pagamento()
        {
            var carrinho = await _comprasBffService.ObterCarrinho();
            if (carrinho.Itens.Count == 0) return RedirectToAction("Index", "Carrinho");

            var pedido = _comprasBffService.MapearParaPedido(carrinho, null);

            return View(pedido);
        }

        [HttpPost]
        [Route("finalizar-pedido")]
        public async Task<IActionResult> FinalizarPedido(PedidoTransacaoViewModel pedidoTransacao)
        {
            if (!ModelState.IsValid) return View("Pagamento", _comprasBffService.MapearParaPedido(
                await _comprasBffService.ObterCarrinho(), null));

            var retorno = await _comprasBffService.FinalizarPedido(pedidoTransacao);

            if (ResponsePossuiErros(retorno))
            {
                var carrinho = await _comprasBffService.ObterCarrinho();
                if (carrinho.Itens.Count == 0) return RedirectToAction("Index", "Carrinho");

                var pedidoMap = _comprasBffService.MapearParaPedido(carrinho, null);
                return View("Pagamento", pedidoMap);
            }

            return RedirectToAction("PedidoConcluido");
        }

        [HttpGet]
        [Route("pedido-concluido")]
        public async Task<IActionResult> PedidoConcluido()
        {
            return View("ConfirmacaoPedido", await _comprasBffService.ObterUltimoPedido());
        }

        [HttpGet("meus-pedidos")]
        public async Task<IActionResult> MeusPedidos()
        {
            return View(await _comprasBffService.ObterListaPorClienteId());
        }

    }
}