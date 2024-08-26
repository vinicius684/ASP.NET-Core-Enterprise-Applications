using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace NSE.WebApp.MVC.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)//(parâmetro) Tipo de autentiação
                .AddCookie(options => 
                {
                    options.LoginPath = "/login"; //quando nãoe stiver logado e eu quiser encaminhar ele para alguma área da minha app
                    options.AccessDeniedPath = "/acesso-negado"; //usuário navegar para um área onde ele não tem acesso

                });

            return services;
        }

        public static void UseIdentityConfiguration(this IApplicationBuilder app)
        { 
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
