
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using NSE.Pedidos.API.Application.Queries;
using NSE.Pedidos.Infra.Migrations;

namespace NSE.Pedidos.API.Services
{
    /*
        BackgroundService: é uma classe que implementa uma interface IHostedService e IDisposable

        -> IHostedService vai definir o StartAsync e StopAsync, como se fosse uma tarefa. Só que quem implementa esse start e stop é a própria classe BackgroundServices, que tb define o ExecuteAsync.
        Portamto, vamos herdar diretamente de IHostedService e IDisposable, porque pelo que entendi não vamos precisar do ExecuteAsync aqui e poderemos implementar manualmente Start e Stop

        É um background service, só que ao invés de ter  um ExecuteAsync tem um star stop, implementados ele de forma mais primitiva.
     */

    public class PedidoOrquestradorIntegrationHandler : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PedidoOrquestradorIntegrationHandler> _logger;
        private Timer _timer;

        public PedidoOrquestradorIntegrationHandler(ILogger<PedidoOrquestradorIntegrationHandler> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)//Gatilho de Inicialização: Inicialização da app por parte do desenvolvedor(HostedService é singleton). Tb posso chamar explicitamente no código
        {
            _logger.LogInformation("Serviço de pedidos iniciado.");

            _timer = new Timer(ProcessarPedidos, null, TimeSpan.Zero,//Chamar o processar Pedidos, gerenciamento de estado null, sem delay, de quanto em quanto tempo vai rodar
               TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }
        private async void ProcessarPedidos(object state)//argumento é estado que vai ser controlado pelo timer 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var pedidoQueries = scope.ServiceProvider.GetRequiredService<IPedidoQueries>();
                var pedido = await pedidoQueries.ObterPedidosAutorizados(); //vai processar 1 pedido a cada 15 s, se fosse um volume mto grande pedidos, tb poderia processar vários por vez, a fila vai obedecer o caminho natural tb

                if (pedido == null) return;

                var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                var pedidoAutorizado = new PedidoAutorizadoIntegrationEvent(pedido.ClienteId, pedido.Id,
                    pedido.PedidoItems.ToDictionary(p => p.ProdutoId, p => p.Quantidade));

                await bus.PublishAsync(pedidoAutorizado);

                _logger.LogInformation($"Pedido ID: {pedido.Id} foi encaminhado para baixa no estoque.");
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)//Gatilho de parada: encerramento da aplicação por parte do desenvolvedor(IHostedService é singleton). Tb pode ser chamado exlicitamente no código
        {
            _logger.LogInformation("Serviço de pedidos finalizado.");

            _timer?.Change(Timeout.Infinite, 0);//quando o serviço é finalizado, zerar o timer, continuar parado pra sempre

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();//ajudar na limpeza de memória, usar o garbage collector. Também pra caso queira ter instruções para caso queira chamar o Stop por algum motivo(garante o funcionamento correto), criar derrepente um dashboard pra iniciar o 
        }
    }
}
