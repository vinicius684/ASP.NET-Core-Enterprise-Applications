using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages;
using NSE.Pedidos.API.Application.DTO;
using NSE.Pedidos.API.Application.Events;
using NSE.Pedidos.Domain;
using NSE.Pedidos.Domain.Pedidos;
using NSE.Pedidos.Domain.Specs;

namespace NSE.Pedidos.API.Application.Commands
{
    public class PedidoCommandHandler : CommandHandler,
        IRequestHandler<AdicionarPedidoCommand, ValidationResult>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IVoucherRepository _voucherRepository;

        public PedidoCommandHandler(IVoucherRepository voucherRepository, 
                                    IPedidoRepository pedidoRepository)
        {
            _voucherRepository = voucherRepository;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<ValidationResult> Handle(AdicionarPedidoCommand message, CancellationToken cancellationToken)
        {
            // Validação do comando
           

            // Mapear Pedido 


            // Aplicar voucher se houver
      

            // Validar pedido - validar o pedido como um todo comparando com os valores vindos de carrinho
      

            // Processar pagamento
      

            // Se pagamento tudo ok!


            // Adicionar Evento


            // Adicionar Pedido Repositorio
 

            // Persistir dados de pedido e voucher(debitar)
            throw new System.NotImplementedException();
        }

        //private Pedido MapearPedido(AdicionarPedidoCommand message)
        //{
        //    var endereco = new Endereco
        //    {
        //        Logradouro = message.Endereco.Logradouro,
        //        Numero = message.Endereco.Numero,
        //        Complemento = message.Endereco.Complemento,
        //        Bairro = message.Endereco.Bairro,
        //        Cep = message.Endereco.Cep,
        //        Cidade = message.Endereco.Cidade,
        //        Estado = message.Endereco.Estado
        //    };

        //    var pedido = new Pedido(message.ClienteId, message.ValorTotal, message.PedidoItems.Select(PedidoItemDTO.ParaPedidoItem).ToList(),
        //        message.VoucherUtilizado, message.Desconto);

        //    pedido.AtribuirEndereco(endereco);
        //    return pedido;
        //}

        //private async Task<bool> AplicarVoucher(AdicionarPedidoCommand message, Pedido pedido)
        //{
        //    if (!message.VoucherUtilizado) return true;

        //    var voucher = await _voucherRepository.ObterVoucherPorCodigo(message.VoucherCodigo);
        //    if (voucher == null)
        //    {
        //        AdicionarErro("O voucher informado não existe!");
        //        return false;
        //    }

        //    var voucherValidation = new VoucherValidation().Validate(voucher);
        //    if (!voucherValidation.IsValid)
        //    {
        //        voucherValidation.Errors.ToList().ForEach(m => AdicionarErro(m.ErrorMessage));
        //        return false;
        //    }

        //    pedido.AtribuirVoucher(voucher);
        //    voucher.DebitarQuantidade();

        //    _voucherRepository.Atualizar(voucher);

        //    return true;
        //}

        //private bool ValidarPedido(Pedido pedido)
        //{
        //    var pedidoValorOriginal = pedido.ValorTotal;
        //    var pedidoDesconto = pedido.Desconto;

        //    pedido.CalcularValorPedido();

        //    if (pedido.ValorTotal != pedidoValorOriginal)
        //    {
        //        AdicionarErro("O valor total do pedido não confere com o cálculo do pedido");
        //        return false;
        //    }

        //    if (pedido.Desconto != pedidoDesconto)
        //    {
        //        AdicionarErro("O valor total não confere com o cálculo do pedido");
        //        return false;
        //    }

        //    return true;
        //}

        //public bool ProcessarPagamento(Pedido pedido)
        //{
        //    return true;
        //}
    }
}