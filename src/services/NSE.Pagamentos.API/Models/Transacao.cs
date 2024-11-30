using System;
using NSE.Core.DomainObjects;

namespace NSE.Pagamentos.API.Models
{
    public class Transacao : Entity //remete um pouco ao obj transaction do NerdsPag, mas aqui vamos salvar apenas auilo que for necessário
    {
        public string CodigoAutorizacao { get; set; }
        public string BandeiraCartao { get; set; }
        public DateTime? DataTransacao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal CustoTransacao { get; set; }
        public StatusTransacao Status { get; set; }
        public string TID { get; set; } // Id da transação. Ex: Já foi autorizado, agora quero capturar, vou usar o TID (é um padrão)
        public string NSU { get; set; } // Meio de captura (paypal)

        public Guid PagamentoId { get; set; }

        // EF Relation
        public Pagamento Pagamento { get; set; }
    }
}