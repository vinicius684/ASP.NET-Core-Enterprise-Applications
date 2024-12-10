using System;
using System.Collections.Generic;

namespace NSE.Core.Messages.Integration
{
    public class PedidoAutorizadoIntegrationEvent : IntegrationEvent
    {
        public Guid ClienteId { get; private set; }
        public Guid PedidoId { get; private set; }
        public IDictionary<Guid, int> Itens { get; private set; }//dictionary representando id e quantidade do item. Não quis criar uma classe nem nada porque tem que ser simples mesmo

        public PedidoAutorizadoIntegrationEvent(Guid clienteId, Guid pedidoId, IDictionary<Guid, int> itens)
        {
            ClienteId = clienteId;
            PedidoId = pedidoId;
            Itens = itens;
        }
    }
}