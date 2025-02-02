using System.Threading.Tasks;
using Grpc.Core;
using NSE.Carrinho.API.Service.gRPC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSE.Carrinho.API.Data;
using NSE.Carrinho.API.Model;
using NSE.WebAPI.Core.Usuario;

namespace NSE.Carrinho.API.Services.gRPC
{
    [Authorize]//authorize pois não é acessivel por alguem que não está logado
    public class CarrinhoGrpcService : CarrinhoCompras.CarrinhoComprasBase
    {
        private readonly ILogger<CarrinhoGrpcService> _logger;//injetado só porque quer fazer um log mesmo, saber que bateu ali e informou em algum lugar

        private readonly IAspNetUser _user;
        private readonly CarrinhoContext _context;

        public CarrinhoGrpcService(
            ILogger<CarrinhoGrpcService> logger,
            IAspNetUser user,
            CarrinhoContext context)
        {
            _logger = logger;
            _user = user;
            _context = context;
        }

        public override async Task<CarrinhoClienteResponse> ObterCarrinho(ObterCarrinhoRequest request, ServerCallContext context)//requisição caso tenho alguma coisa la dentro e context não precisa se preocupar com ele, está pra fazer o controle
        {
            _logger.LogInformation("Chamando ObterCarrinho");

            var carrinho = await ObterCarrinhoCliente() ?? new CarrinhoCliente();

            return MapCarrinhoClienteToProtoResponse(carrinho);
        }

        private async Task<CarrinhoCliente> ObterCarrinhoCliente()
        {
            return await _context.CarrinhoCliente
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.ClienteId == _user.ObterUserId());
        }

        private static CarrinhoClienteResponse MapCarrinhoClienteToProtoResponse(CarrinhoCliente carrinho)//pode usar o autoMapper, mas vai dar tanto trabalho pra configurar que edu prefere fazer na mão
        {
            var carrinhoProto = new CarrinhoClienteResponse
            {
                Id = carrinho.Id.ToString(),
                Clienteid = carrinho.ClienteId.ToString(),
                Valortotal = (double)carrinho.ValorTotal,//proto só aceita double
                Desconto = (double)carrinho.Desconto,
                Voucherutilizado = carrinho.VoucherUtilizado,
            };

            if (carrinho.Voucher != null)
            {
                carrinhoProto.Voucher = new VoucherResponse
                {
                    Codigo = carrinho.Voucher.Codigo,
                    Percentual = (double?)carrinho.Voucher.Percentual ?? 0,//se for nulo retorna 0, pq o Proto não ceita nullable
                    Valordesconto = (double?)carrinho.Voucher.ValorDesconto ?? 0,
                    Tipodesconto = (int)carrinho.Voucher.TipoDesconto//enum pra int
                };
            }

            foreach (var item in carrinho.Itens)
            {
                carrinhoProto.Itens.Add(new CarrinhoItemResponse
                {
                    Id = item.Id.ToString(),
                    Nome = item.Nome,
                    Imagem = item.Imagem,
                    Produtoid = item.ProdutoId.ToString(),
                    Quantidade = item.Quantidade,
                    Valor = (double)item.Valor
                });
            }

            return carrinhoProto;
        }
    }
}