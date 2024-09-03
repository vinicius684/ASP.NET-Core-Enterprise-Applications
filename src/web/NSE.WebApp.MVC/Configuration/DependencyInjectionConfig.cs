using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>(); //forma de configurar e gerenciar instancias HttpClient

            services.AddHttpClient<ICatalogoService, CatalogoService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //singleton porque trata-se de uma ferramenta que chama o HttpContext que é scoped e trata cada usuário(requisição) como unico
            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
