using NSE.Identidade.API.Extensions;

namespace NSE.WebApp.MVC.Configuration
{
    public static class WebAppConfig
    {
        public static IServiceCollection AddMvcConfiguration(this IServiceCollection services) //ExtensionMethod de Services
        {
            services.AddControllersWithViews();

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this WebApplication app, IWebHostEnvironment env) //Exthensionmethod de app
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/erro/500");//Captura exceções não tratadas que podem ocorrer durante a execução da requisição e redireciona para uma página de erro genérica
                app.UseStatusCodePagesWithRedirects("/erro/{0}");//Redireciona a resposta HTTP com base no código de status.

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<ExceptionMiddleware>();//sendo declarado após os middlewares de redirecionamento gnéricos, pois no response os middlewares são executados na ordem iversa

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );


            return app;
        }
    }
}
