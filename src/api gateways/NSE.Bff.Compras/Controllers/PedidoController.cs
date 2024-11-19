using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Bff.Compras.Models;
using NSE.Bff.Compras.Services;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Bff.Compras.Controllers
{
    [Authorize]
    public class PedidoController : MainController
    {
        private readonly ICatalogoService _catalogoService;
        private readonly ICarrinhoService _carrinhoService;
        private readonly IPedidoService _pedidoService;
        private readonly IClienteService _clienteService;

        public PedidoController(
            ICatalogoService catalogoService,
            ICarrinhoService carrinhoService,
            IPedidoService pedidoService,
            IClienteService clienteService)
        {
            _catalogoService = catalogoService;
            _carrinhoService = carrinhoService;
            _pedidoService = pedidoService;
            _clienteService = clienteService;
        }

        [HttpPost]
        [Route("compras/pedido")]
        public async Task<IActionResult> AdicionarPedido(PedidoDTO pedido)
        {
            /*Quando fechar o Pedido, não posso simplesmente pensar que o carrinhoe está perfeito, estoque pode ter acabado, preço pode ter mudado, por isso estou obtendo os itens do caralogo*/
            var carrinho = await _carrinhoService.ObterCarrinho();
            var produtos = await _catalogoService.ObterItens(carrinho.Itens.Select(p => p.ProdutoId));
            var endereco = await _clienteService.ObterEndereco();

            if (!await ValidarCarrinhoProdutos(carrinho, produtos)) return CustomResponse();

            PopularDadosPedido(carrinho, endereco, pedido);//adicionando infos do endereco e carrinho ao Pedido (que ja tem as infos de pagamento)

            return CustomResponse(await _pedidoService.FinalizarPedido(pedido));
        }

        [HttpGet("compras/pedido/ultimo")]
        public async Task<IActionResult> UltimoPedido()
        {
            var pedido = await _pedidoService.ObterUltimoPedido();
            if (pedido is null)
            {
                AdicionarErroProcessamento("Pedido não encontrado!");
                return CustomResponse();
            }

            return CustomResponse(pedido);
        }

        [HttpGet("compras/pedido/lista-cliente")]
        public async Task<IActionResult> ListaPorCliente()
        {
            var pedidos = await _pedidoService.ObterListaPorClienteId();

            return pedidos == null ? NotFound() : CustomResponse(pedidos);
        }

        private async Task<bool> ValidarCarrinhoProdutos(CarrinhoDTO carrinho, IEnumerable<ItemProdutoDTO> produtos)
        {
            if (carrinho.Itens.Count != produtos.Count())
            {
                var itensIndisponiveis = carrinho.Itens.Select(c => c.ProdutoId).Except(produtos.Select(p => p.Id)).ToList();//except pra desconbrir quais produtos estão indisponíveis

                foreach (var itemId in itensIndisponiveis)//pra cada item indisponível em catalogo
                {
                    var itemCarrinho = carrinho.Itens.FirstOrDefault(c => c.ProdutoId == itemId);//vou obter eles do carrinho através do id pra poder identificar o nome e adicionar um erro
                    AdicionarErroProcessamento($"O item {itemCarrinho.Nome} não está mais disponível no catálogo, o remova do carrinho para prosseguir com a compra");
                }

                return false;
            }
            //caso todos os itens do carrinho estejam disponíveis no catalogo
            foreach (var itemCarrinho in carrinho.Itens)//vou dar um foreach nos itens do carrinho
            {
                var produtoCatalogo = produtos.FirstOrDefault(p => p.Id == itemCarrinho.ProdutoId);//pegando cada um desses itens e buscando eles nos produtos do catalogo

                if (produtoCatalogo.Valor != itemCarrinho.Valor)//se o valor doproduto for diferente do valor do item
                {
                    var msgErro = $"O produto {itemCarrinho.Nome} mudou de valor (de: " +
                                  $"{string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", itemCarrinho.Valor)} para: " +
                                  $"{string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produtoCatalogo.Valor)}) desde que foi adicionado ao carrinho.";

                    AdicionarErroProcessamento(msgErro);//vou adicionar erro de processamento 

                    var responseRemover = await _carrinhoService.RemoverItemCarrinho(itemCarrinho.ProdutoId);//vou remover esse item do carrinho
                    if (ResponsePossuiErros(responseRemover))
                    {
                        AdicionarErroProcessamento($"Não foi possível remover automaticamente o produto {itemCarrinho.Nome} do seu carrinho, _" +
                                                   "remova e adicione novamente caso ainda deseje comprar este item");
                        return false;
                    }

                    itemCarrinho.Valor = produtoCatalogo.Valor;//item recebe o novo valor de produto e é adicionado novamente ao carrinho
                    var responseAdicionar = await _carrinhoService.AdicionarItemCarrinho(itemCarrinho);

                    if (ResponsePossuiErros(responseAdicionar))
                    {
                        AdicionarErroProcessamento($"Não foi possível atualizar automaticamente o produto {itemCarrinho.Nome} do seu carrinho, _" +
                                                   "adicione novamente caso ainda deseje comprar este item");
                        return false;
                    }

                    LimparErrosProcessamento();//limpar erros
                    AdicionarErroProcessamento(msgErro + " Atualizamos o valor em seu carrinho, realize a conferência do pedido e se preferir remova o produto");//adicionar todas as infos e uma só 

                    return false;
                }
            }

            return true;
        }
        
        private void PopularDadosPedido(CarrinhoDTO carrinho, EnderecoDTO endereco, PedidoDTO pedido)
        {
            pedido.VoucherCodigo = carrinho.Voucher?.Codigo;
            pedido.VoucherUtilizado = carrinho.VoucherUtilizado;
            pedido.ValorTotal = carrinho.ValorTotal;
            pedido.Desconto = carrinho.Desconto;
            pedido.PedidoItems = carrinho.Itens;

            pedido.Endereco = endereco;
        }
    }
}
