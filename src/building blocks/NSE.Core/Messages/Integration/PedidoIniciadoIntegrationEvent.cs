using System;

namespace NSE.Core.Messages.Integration
{
    public class PedidoIniciadoIntegrationEvent : IntegrationEvent
    {
        /*
            Preciso dizer que aquele pedido é de tal cliente, de tal pedido

            Se o gateway vai exigir dados do cliente, cnpj, cpf, endereco, e vai. Tb vou passar essas informações aqui
         */
        public Guid ClienteId { get; set; }
        public Guid PedidoId { get; set; }
        public int TipoPagamento { get; set; }
        public decimal Valor { get; set; }//valor total já descontando voucher, adicionando taxas de entrega, correios essas coisas

        public string NomeCartao { get; set; }
        public string NumeroCartao { get; set; }
        public string MesAnoVencimento { get; set; }
        public string CVV { get; set; }
    }
}