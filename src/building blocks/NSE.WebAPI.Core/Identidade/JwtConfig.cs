using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NSE.WebAPI.Core.Identidade
{
    public static class JwtConfig //configs de como ler um token
    {
        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services,
           IConfiguration configuration)
        {
            
            // JWT
            var appSettingsSection = configuration.GetSection("AppSettings");//atraves da classe configuration vou obter uma seção do app settings
            services.Configure<AppSettings>(appSettingsSection);//"pedindo" classe AppSettings represente os dados da seção obtida

            var appSettings = appSettingsSection.Get<AppSettings>();//através da section, obtendo a classe já populada

            services.AddAuthentication(options =>//Dizendo que tanto a forma de autenticar, quanto o "desafio" de como apresentar e credenciar um usuário é feito internamente dependem do Padrão JWT, poderia usar via cookie, via outros providers...
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>//add suporte  pra esse tipo específico de token e opções
            {
                bearerOptions.RequireHttpsMetadata = true;
                bearerOptions.SaveToken = true;
                bearerOptions.SetJwksOptions(new JwkOptions(appSettings.AutenticacaoJwksUrl));//olha, vai entender um JWT e pra ententer como ele funciona, vai precisar pegar a url desse endpoint e ir lá consultar chave publica pra entender a validar o Token
                bearerOptions.MapInboundClaims = false;
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
