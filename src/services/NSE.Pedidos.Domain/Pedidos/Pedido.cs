using System;
using System.Collections.Generic;
using System.Linq;
using NSE.Core.DomainObjects;

namespace NSE.Pedidos.Domain.Pedidos
{
    public class Pedido : Entity, IAggregateRoot
    {
        public Pedido(Guid clienteId, decimal valorTotal, List<PedidoItem> pedidoItems,
            bool voucherUtilizado = false, decimal desconto = 0, Guid? voucherId = null)
        {
            ClienteId = clienteId;
            ValorTotal = valorTotal;
            _pedidoItems = pedidoItems;

            Desconto = desconto;
            VoucherUtilizado = voucherUtilizado;
            VoucherId = voucherId;
        }

        // EF ctor
        protected Pedido() { }

        public int Codigo { get; private set; } //vamos configurar para o EF gerar uma sequencia
        public Guid ClienteId { get; private set; }
        public Guid? VoucherId { get; private set; }
        public bool VoucherUtilizado { get; private set; }
        public decimal Desconto { get; private set; }
        public decimal ValorTotal { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }

        private readonly List<PedidoItem> _pedidoItems;
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;//oferecendo a lista dessa forma, só é possível ler. Qualquer alteração que quiser fazer terá que ser feita via método. Cuidado para que a propriedade não seja manipulada no sentido de setar uma nova coleção, mas tb no sentido de manipular os itens que estão lá dentro (só o set privado não garante esse último)

        public Endereco Endereco { get; private set; }//obj valor

        // EF Rel.
        public Voucher Voucher { get; private set; }

        public void AutorizarPedido()
        {
            PedidoStatus = PedidoStatus.Autorizado;
        }

        public void AtribuirVoucher(Voucher voucher)
        {
            VoucherUtilizado = true;
            VoucherId = voucher.Id;
            Voucher = voucher;
        }

        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        public void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(p => p.CalcularValor());
            CalcularValorTotalDesconto();
        }

        public void CalcularValorTotalDesconto()
        {
            if (!VoucherUtilizado) return;

            decimal desconto = 0;
            var valor = ValorTotal;

            if (Voucher.TipoDesconto == TipoDescontoVoucher.Porcentagem)
            {
                if (Voucher.Percentual.HasValue)
                {
                    desconto = (valor * Voucher.Percentual.Value) / 100;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }

            ValorTotal = valor < 0 ? 0 : valor;
            Desconto = desconto;
        }
    }
}