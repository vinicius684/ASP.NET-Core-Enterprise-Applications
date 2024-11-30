using System;
using System.Collections.Generic;
using NSE.Core.DomainObjects;

namespace NSE.Pagamentos.API.Models
{
    /*
        Classe principal, Pagamento em si
        
        Mais serve pra persistencia do que pra regra de negócio, portanto apenas um ou outro conceito do DDD

     */
    public class Pagamento : Entity, IAggregateRoot 
    {
        public Pagamento()
        {
            Transacoes = new List<Transacao>();
        }

        public Guid PedidoId { get; set; }
        public TipoPagamento TipoPagamento { get; set; }
        public decimal Valor { get; set; }

        public CartaoCredito CartaoCredito { get; set; }//está presente no pagamento, mas não vamos persistir(declarado no mappings pra ignorar). Está aqui mais pra ajudar no momento do pagamento.

        // EF Relation
        public ICollection<Transacao> Transacoes { get; set; }//Pagamento é feito em duas etapas. Por isso uma lista de Transação, Transação para a Autorização e outra para a Captura, e se houver chargerback há ainda uma terceira

        public void AdicionarTransacao(Transacao transacao)
        {
            Transacoes.Add(transacao);
        }
    }
}