using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Carrinho.API.Data;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;

namespace NSE.Carrinho.API.Services
{
    public class CarrinhoIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;//pra ler a fila
        private readonly IServiceProvider _serviceProvider;//pq vamos pecisar instanciar objetos que são resolvidos de modo scoped em um modo singleton

        public CarrinhoIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void SetSubscribers() //método privado par setar meus subscribers
        {
            _bus.SubscribeAsync<PedidoRealizadoIntegrationEvent>("PedidoRealizado", async request =>
                await ApagarCarrinho(request)); //request é a própria mensagem que vou ficar escutando para receber
        }

        //esse método não precisa estar aqui, poderia estar em um serviço, uma classe sespecializada, mas como nossa arquitetura é super-simples..
        private async Task ApagarCarrinho(PedidoRealizadoIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CarrinhoContext>();

            var carrinho = await context.CarrinhoCliente
                .FirstOrDefaultAsync(c => c.ClienteId == message.ClienteId);

            if (carrinho != null)
            {
                context.CarrinhoCliente.Remove(carrinho);
                await context.SaveChangesAsync();
            }
        }
    }
}