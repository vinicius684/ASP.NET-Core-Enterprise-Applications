using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Identidade.API.Services;
using NSE.WebAPI.Core.Identidade;
using NSE.WebAPI.Core.Usuario;

namespace NSE.Identidade.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services) //ExtensionMethod de Services
        {
            services.AddControllers();

            services.AddScoped<AuthenticationService>();
            services.AddScoped<IAspNetUser, AspNetUser>();//utilizando basicamnte pra obter endpoint da api, no CodificarToken

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this WebApplication app, IWebHostEnvironment env) //Exthensionmethod de app
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();//utilize esquema de rotas

            app.UseAuthConfiguration(); //ExtensionMethod de Useidentity está aqui pois  app.UseAuthentication(); app.UseAuthorization() precisam estar extamente entre UseRouting e MapControllers

            app.MapControllers();

            
            //Endpoint personalizado: dominio ou localhost:porta/jwks
            //Endpoint default: localhost:porta/jwks
            app.UseJwksDiscovery();//reposnsavel por expor o endpoint da minha chave pública

            return app;
        }
    }
}

