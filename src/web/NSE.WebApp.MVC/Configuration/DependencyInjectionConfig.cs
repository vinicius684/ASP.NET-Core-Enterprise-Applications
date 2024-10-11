using Microsoft.AspNetCore.Mvc.DataAnnotations;
using NSE.WebAPI.Core.Usuario;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IValidationAttributeAdapterProvider, CpfValidationAttributeAdapterProvider>();//adapter provider que está sendo resolvido em tempo de execução pelo próprioo Razor

            #region HttpServices

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();//transient(instancia por solicitação do serviço) pois já estou trabalhndo no modo scoped do request HttpClient

            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                 .AddPolicyHandler(PollyExtensions.EsperarTentar())
                 .AddTransientHttpErrorPolicy(
                     p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICatalogoService, CatalogoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() //colocando o Handler para manipular meu request do HttpClient                                                           
                                                                                   //.AddTransientHttpErrorPolicy( 
                                                                                   // p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600))); 1 - Policy simples Padrão
                 .AddPolicyHandler(PollyExtensions.EsperarTentar()) //2 - Policy Handle
                 .AddTransientHttpErrorPolicy(
                     p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));//Circuit Breaker - Parâmetros n vezs que a app deve falhar(pega multiplos usuários) e o tempo que deve esperar até tentar novamente

            services.AddHttpClient<IComprasBffService, ComprasBffService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                 .AddPolicyHandler(PollyExtensions.EsperarTentar())
                 .AddTransientHttpErrorPolicy(
                     p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            #endregion


            #region Refit - facilita httpcliente get 
            //Refit para ICatalogoService
            //services.AddHttpClient("Refit", options =>
            //    {
            //        options.BaseAddress = new Uri(configuration.GetSection("CatalogoUrl").Value);
            //    })
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() //colocando o Handler para manipular meu request do HttpClient 
            //    .AddTypedClient(Refit.RestService.For<ICatalogoServiceRefit>);
            #endregion


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //singleton porque trata-se de uma ferramenta que chama o HttpContext que é scoped e trata cada usuário(requisição) como unico
            services.AddScoped<IAspNetUser, AspNetUser>();
        }
    }

    public class PollyExtensions
    {
        public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
        {
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                }, (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Tentando pela {retryCount} vez!");
                    Console.ForegroundColor = ConsoleColor.White;
                });

            return retry;
        }
    }
}
