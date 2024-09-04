using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;
using Polly;
using Polly.Extensions.Http;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();//transient(instancia por solicitação) pois já estou trabalhndo no modo scoped do request HttpClient

            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>(); //forma de configurar e gerenciar instancias HttpClient

            var retryWaitPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Tentando pela {retryCount} vez!");
                    Console.ForegroundColor = ConsoleColor.White;
                });

            services.AddHttpClient<ICatalogoService, CatalogoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() //colocando o Handler para manipular meu request do HttpClient                                                           
                    //.AddTransientHttpErrorPolicy( 
                    // p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600))); Policy Padrão
                    .AddPolicyHandler(retryWaitPolicy); //Policy Handle
   
            #region Refit
            //Refit para ICatalogoService
            //services.AddHttpClient("Refit", options =>
            //    {
            //        options.BaseAddress = new Uri(configuration.GetSection("CatalogoUrl").Value);
            //    })
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() //colocando o Handler para manipular meu request do HttpClient 
            //    .AddTypedClient(Refit.RestService.For<ICatalogoServiceRefit>);
            #endregion


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //singleton porque trata-se de uma ferramenta que chama o HttpContext que é scoped e trata cada usuário(requisição) como unico
            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
