using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Catalogo.API.Models;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;

namespace NSE.Catalogo.API.Services
{
    public class CatalogoIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public CatalogoIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void SetSubscribers()
        {
            _bus.SubscribeAsync<PedidoAutorizadoIntegrationEvent>("PedidoAutorizado", async request =>
                await BaixarEstoque(request));
        }

        //código bem simples a ideia é ser simples. É um modelo de MS
        private async Task BaixarEstoque(PedidoAutorizadoIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var produtosComEstoque = new List<Produto>();
                var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

                var idsProdutos = string.Join(",", message.Itens.Select(c => c.Key));//pegando o key dos produtos e dando um string.Join deparando-os por vírgular
                var produtos = await produtoRepository.ObterProdutosPorId(idsProdutos);

                if (produtos.Count != message.Itens.Count)
                {
                    CancelarPedidoSemEstoque(message);
                    return;
                }

                foreach (var produto in produtos)
                {
                    var quantidadeProduto = message.Itens.FirstOrDefault(p => p.Key == produto.Id).Value;//seleção dentro do IDictionary da minha mensagem, onde a chave = id. Ou seja, pegando o id do produto que veio do catalogo, comparando com o mesmo Produto que está lá no Pedido, pra pegar a quantidade de itens daquele pedido
                    
                    if (produto.EstaDisponivel(quantidadeProduto))
                    {
                        produto.RetirarEstoque(quantidadeProduto);
                        produtosComEstoque.Add(produto);
                    }
                }

                if (produtosComEstoque.Count != message.Itens.Count)
                {
                    CancelarPedidoSemEstoque(message);
                    return;
                }

                foreach (var produto in produtosComEstoque)
                {
                    produtoRepository.Atualizar(produto);//ChangeTracker(AsNoTraking) está desligado para a consulta ObterPodutosProId
                }

                if (!await produtoRepository.UnitOfWork.Commit())
                {
                    throw new DomainException($"Problemas ao atualizar estoque do pedido {message.PedidoId}");//Quando soltar essa exception, a mensagem que estaria sendo trabalhada aqui, não vai ser maarcada como completada e vai voltar pra fila pra ser manipulada posteriormente
                    /*
                        Alerta de boas práticas relativo ao uso das  exception no meio do tratamento de mensagens que vem da fila
                        -> Ao lançar uma exception durante o processamento de uma mensagem na fila, esta mensagem é encaminhada para 
                           uma fila de erros e por padrão será executada novamente até obter sucesso
                        ->É necessário analisar se lançar exceptions é a melhor alternativa, pois a execução posterior de mensagens acumuladas podem ocasionar comportamentos idnesejados. Ex: Pedido que foi feito depois vai retirar os itens do estoque e o que foi feito antes não vai ter estoque...
                        
                        As vezes em detrimento da exception, a melhor opção é salvar um log de erro, daí vai ter um dashboard pra ver esse erro e poderá tratar o erro pontualmente
                     */
                }

                var pedidoBaixado = new PedidoBaixadoEstoqueIntegrationEvent(message.ClienteId, message.PedidoId);
                await _bus.PublishAsync(pedidoBaixado);
            }
        }

        public async void CancelarPedidoSemEstoque(PedidoAutorizadoIntegrationEvent message)
        {
            var pedidoCancelado = new PedidoCanceladoIntegrationEvent(message.ClienteId, message.PedidoId);
            await _bus.PublishAsync(pedidoCancelado);
        }
    }
}