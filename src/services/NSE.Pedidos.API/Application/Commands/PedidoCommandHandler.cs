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
using NSE.Pedidos.Domain.Vouchers.Specs;


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

        /*
            Poderia ser resolvido com um serviço de domínio(Receberia o comando, montaria a entidade e chamaria serviço de domínio), poderia ser uma Saga, poderia ser um serviço de aplicação... Não há forma 100% correta de se fazer
         */
        public async Task<ValidationResult> Handle(AdicionarPedidoCommand message, CancellationToken cancellationToken)
        {
            // Validação do comando
            if (!message.EhValido()) return message.ValidationResult; //nesse momento sim faz sentido retonar mensagens especificas de erro do voucher

            // Mapear Pedido 
            var pedido = MapearPedido(message);

            // Aplicar voucher se houver
            if (!await AplicarVoucher(message, pedido)) return ValidationResult;

            // Validar pedido - validar o pedido como um todo comparando com os valores vindos de carrinho
            if (!ValidarPedido(pedido)) return ValidationResult;

            // Processar pagamento
            if (!ProcessarPagamento(pedido)) return ValidationResult;

            // Se pagamento tudo ok!
            pedido.AutorizarPedido();

            // Adicionar Evento
            pedido.AdicionarEvento(new PedidoRealizadoEvent(pedido.Id, pedido.ClienteId));

            // Adicionar Pedido Repositorio
            _pedidoRepository.Adicionar(pedido);

            // Persistir dados de pedido e voucher(debitar)
            return await PersistirDados(_pedidoRepository.UnitOfWork);

            /*
                Esse último passo, mto importante de entender.
                Está persistindo tanto Pedido quanto Voucher, não precisa do unit of work tb do voucher, pois o UOF trabalha com a instancia do contexto dentro do request. 
                Qualquer instancia que esteja dentro desse context será persistida nesse ponto, não importa qual repository

                Inclusive se chamasse PersistirDados duas vezes, daria problema. Persistiria ambos e depois não teria nada pra persistir
             */
        }

        private Pedido MapearPedido(AdicionarPedidoCommand message)
        {
            var endereco = new Endereco
            {
                Logradouro = message.Endereco.Logradouro,
                Numero = message.Endereco.Numero,
                Complemento = message.Endereco.Complemento,
                Bairro = message.Endereco.Bairro,
                Cep = message.Endereco.Cep,
                Cidade = message.Endereco.Cidade,
                Estado = message.Endereco.Estado
            };

            var pedido = new Pedido(message.ClienteId, message.ValorTotal, message.PedidoItems.Select(PedidoItemDTO.ParaPedidoItem).ToList(),
                message.VoucherUtilizado, message.Desconto);

            pedido.AtribuirEndereco(endereco);
            return pedido;
        }

        private async Task<bool> AplicarVoucher(AdicionarPedidoCommand message, Pedido pedido)
        {
            if (!message.VoucherUtilizado) return true;//se não tem voucher retorna true

            var voucher = await _voucherRepository.ObterVoucherPorCodigo(message.VoucherCodigo);//caso tenha, vou obter esse voucher pelo código
            if (voucher == null)
            {
                AdicionarErro("O voucher informado não existe!");
                return false;
            }

            var voucherValidation = new VoucherValidation().Validate(voucher);//validando voucher e adicionando mensagens caso não esteja válido
            if (!voucherValidation.IsValid)
            {
                voucherValidation.Errors.ToList().ForEach(m => AdicionarErro(m.ErrorMessage));
                return false;
            }

            pedido.AtribuirVoucher(voucher);//
            voucher.DebitarQuantidade();

            _voucherRepository.Atualizar(voucher);

            return true;
        }

        private bool ValidarPedido(Pedido pedido) //diz o edu que está simplória, poderia ir além
        {
            var pedidoValorOriginal = pedido.ValorTotal;
            var pedidoDesconto = pedido.Desconto;

            pedido.CalcularValorPedido();

            if (pedido.ValorTotal != pedidoValorOriginal)
            {
                AdicionarErro("O valor total do pedido não confere com o cálculo do pedido");
                return false;
            }

            if (pedido.Desconto != pedidoDesconto)
            {
                AdicionarErro("O valor total não confere com o cálculo do pedido");
                return false;
            }

            return true;
        }

        public bool ProcessarPagamento(Pedido pedido)
        {
            return true;
        }
    }
}