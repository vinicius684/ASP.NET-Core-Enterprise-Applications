using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.Clientes.API.Services;
using NSE.Core.Utils;
using NSE.MessageBus;

namespace NSE.Clientes.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus")) //Nome da minha connextionString
                .AddHostedService<RegistroClienteIntegrationHandler>(); //poderia ser separado, mas AddMessageBus é um IServiceCollection, por isolamento de responsabilidade-configuração já coloca junto. Se tiver outro Background Service q n tem nada a ver com o bus n faz sentido tá aqui
        }
    }
}