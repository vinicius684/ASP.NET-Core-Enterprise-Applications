using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Identidade.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services) //ExtensionMethod de Services
        {
            services.AddControllers();

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

            return app;
        }
    }
}

