﻿using EasyNetQ;
using EasyNetQ.Internals;
using NSE.Core.Messages.Integration;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace NSE.MessageBus
{
    public class MessageBus : IMessageBus //tenho um message Bus que abstrai o EasyNetQ, EasyNetQ abstrai a própria biblioteca do rabbit MQ pra .net, que abstrai a coneção com o rabbitMQ
    {
        private IBus _bus;//sem readonly pq vai ser manipulado. Sem instanciação no construtor pois já crio instancia no CreateBus
        private IAdvancedBus _advancedBus;

        private readonly string _connectionString; //criar conexão com o buss aqui dentro

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;
            TryConnect();
        }

        public bool IsConnected => _bus?.Advanced.IsConnected ?? false;
        public IAdvancedBus AdvancedBus => _bus?.Advanced;//bus já foi setao, mas n tem problema nenhum obter a mesma propriedade de outro lugar. Dessa forma é interessante pois só vou ter advanced se o bus estiver conectado

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            TryConnect();//tryConnect novamente para o caso de perder a conexão
            _bus.PubSub.Publish(message);
        }

        public async Task PublishAsync<T>(T message) where T : IntegrationEvent
        {
            TryConnect();
            await _bus.PubSub.PublishAsync(message);
        }

        public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
        {
            TryConnect();
            _bus.PubSub.Subscribe(subscriptionId, onMessage);
        }

        public AwaitableDisposable<SubscriptionResult> SubscribeAsync<T>(string subscriptionId,
             Func<T, Task> onMessage) where T : class
        {
            TryConnect();
            return _bus.PubSub.SubscribeAsync(subscriptionId, onMessage);
        }

        public TResponse Request<TRequest, TResponse>(TRequest request) where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.Rpc.Request<TRequest, TResponse>(request);
        }

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent where TResponse : ResponseMessage
        {
            TryConnect();
            return await _bus.Rpc.RequestAsync<TRequest, TResponse>(request);
        }

        public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEvent where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.Rpc.Respond(responder);
        }

        public AwaitableDisposable<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
             where TRequest : IntegrationEvent
                where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.Rpc.RespondAsync(responder);
        }

        private void TryConnect()
        {
            if (IsConnected) return;

            //camada extra de cuidado para conexão - Utilizando Polly
            var policy = Policy.Handle<EasyNetQException>() //se receber uma exception do EasyNetQ
                .Or<BrokerUnreachableException>()//Ou do Exception do RabbitMQ.Cliente
                .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                _bus = RabbitHutch.CreateBus(_connectionString);
                _advancedBus = _bus.Advanced;
                _advancedBus.Disconnected += OnDisconnect;
            });
        }

        private void OnDisconnect(object s, EventArgs e) //essa policy é basicamente para app conectar mais rapido ao event bus
        {
            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .RetryForever(); 

            policy.Execute(TryConnect);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

    }
}
