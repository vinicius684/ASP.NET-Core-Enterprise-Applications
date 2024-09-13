using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.Core.Utils;
using NSE.MessageBus;

namespace NSE.identidade.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguartion(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus")); //Nome da minha connextionString
               
        }
    }
}