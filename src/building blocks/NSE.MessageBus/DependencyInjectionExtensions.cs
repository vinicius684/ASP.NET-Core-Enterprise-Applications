using System;
using Microsoft.Extensions.DependencyInjection;

namespace NSE.MessageBus
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services, string connection)
        {
            if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException(); //não posso receber uma collectionstring vazia

            services.AddSingleton<IMessageBus>(new MessageBus(connection));

            return services;
        }
    }
}