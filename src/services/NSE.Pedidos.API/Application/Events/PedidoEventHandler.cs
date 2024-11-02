﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;

namespace NSE.Pedidos.API.Application.Events
{
    public class PedidoEventHandler : INotificationHandler<PedidoRealizadoEvent>
    {
        private readonly IMessageBus _bus;

        public PedidoEventHandler(IMessageBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(PedidoRealizadoEvent message, CancellationToken cancellationToken)
        {
            await _bus.PublishAsync(new PedidoRealizadoIntegrationEvent(message.ClienteId));//Poderia ter mais de uma api escutando esse evento, mas nesse até o memento é só a Carrinho
        }
    }
}