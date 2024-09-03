using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();//transient(instancia por solicitação) pois já estou trabalhndo no modo scoped do request HttpClient

            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>(); //forma de configurar e gerenciar instancias HttpClient

            services.AddHttpClient<ICatalogoService, CatalogoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>(); //colocando o Handler para manipular meu request do HttpClient 
                ////.AddTransientHttpErrorPolicy(
                ////p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
                //.AddPolicyHandler(PollyExtensions.EsperarTentar())
                //.AddTransientHttpErrorPolicy(
                //    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //singleton porque trata-se de uma ferramenta que chama o HttpContext que é scoped e trata cada usuário(requisição) como unico
            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
