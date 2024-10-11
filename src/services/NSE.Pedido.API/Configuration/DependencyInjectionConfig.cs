using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSE.Core.Mediator;
using NSE.WebAPI.Core.Usuario;

namespace NSE.Pedidos.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // API
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();
            
            //// Commands
            //services.AddScoped<IRequestHandler<AdicionarPedidoCommand, ValidationResult>, PedidoCommandHandler>();

            //// Events
            //services.AddScoped<INotificationHandler<PedidoRealizadoEvent>, PedidoEventHandler>();

            //// Application
            //services.AddScoped<IMediatorHandler, MediatorHandler>();
            //services.AddScoped<IVoucherQueries, VoucherQueries>();
            //services.AddScoped<IPedidoQueries, PedidoQueries>();

            //// Data
            //services.AddScoped<IPedidoRepository, PedidoRepository>();
            //services.AddScoped<IVoucherRepository, VoucherRepository>();
            //services.AddScoped<PedidosContext>();
        }
    }
}