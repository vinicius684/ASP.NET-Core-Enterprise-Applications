using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSE.WebAPI.Core.Identidade
{
    public static class JwtConfig
    {
        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services,
           IConfiguration configuration)
        {
            
            // JWT
            var appSettingsSection = configuration.GetSection("AppSettings");//atraves da classe configuration vou obter uma seção do app settings
            services.Configure<AppSettings>(appSettingsSection);//"pedindo" classe AppSettings represente os dados da seção obtida

            var appSettings = appSettingsSection.Get<AppSettings>();//através da section, obtendo a classe já populada
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);//tranformando minha chave em uma sequencia de bytes no formato ASCII

            services.AddAuthentication(options =>//Dizendo que tanto a forma de autenticar, quanto o "desafio" de como apresentar e credenciar um usuário é feito internamente dependem do Padrão JWT, poderia usar via cookie, via outros providers...
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>//add suporte  pra esse tipo específico de token e opções
            {
                bearerOptions.RequireHttpsMetadata = true;
                bearerOptions.SaveToken = true;
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,//validar o emissor com base na assinatura, não posso utilizar um token qualquer com uma ssinatura qualquer
                    IssuerSigningKey = new SymmetricSecurityKey(key),//assinatura do emissor, segredo do token
                    ValidateIssuer = true,//validar emissor
                    ValidateAudience = true,//validar onde o oken é válido
                                            //ValidAudiences =
                    ValidAudience = appSettings.ValidoEm,
                    ValidIssuer = appSettings.Emissor,
                };
            });
            //

            return services;
        }

        public static IApplicationBuilder UseAuthConfiguration(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
