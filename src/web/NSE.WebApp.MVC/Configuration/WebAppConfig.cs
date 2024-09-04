using Microsoft.AspNetCore.Localization;
using NSE.Identidade.API.Extensions;
using NSE.WebApp.MVC.Extensions;
using System.Globalization;

namespace NSE.WebApp.MVC.Configuration
{
    public static class WebAppConfig
    {
        public static IServiceCollection AddMvcConfiguration(this IServiceCollection services, IConfiguration configuration) //ExtensionMethod de Services
        {
            services.AddControllersWithViews();

            services.Configure<AppSettings>(configuration);

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this WebApplication app, IWebHostEnvironment env) //Exthensionmethod de app
        {
            //if (!env.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/erro/500");//Captura exceções não tratadas que podem ocorrer durante a execução da requisição e redireciona para uma página de erro genérica
            //    app.UseStatusCodePagesWithRedirects("/erro/{0}");//Redireciona a resposta HTTP com base no código de status.

            //    app.UseHsts();
            //}

            app.UseExceptionHandler("/erro/500");//Captura exceções não tratadas que podem ocorrer durante a execução da requisição e redireciona para uma página de erro genérica
            app.UseStatusCodePagesWithRedirects("/erro/{0}");//Redireciona a resposta HTTP com base no código de status.

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityConfiguration();

            var supportedCultures = new[] { new CultureInfo("pt-BR") }; //se quisesse dar suporte a várias culturas, acultura escolhida poderia estar sendo passada via parâmetro na sua rota ou persistida num cookie
            app.UseRequestLocalization(new RequestLocalizationOptions 
            {
                DefaultRequestCulture = new RequestCulture("pt-BR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseMiddleware<ExceptionMiddleware>();//sendo declarado após os middlewares de redirecionamento genéricos, pois no response os middlewares são executados na ordem iversa

            app.MapControllerRoute(
                name: "default",
                 pattern: "{controller=Catalogo}/{action=Index}/{id?}"
            );


            return app;
        }
    }
}
