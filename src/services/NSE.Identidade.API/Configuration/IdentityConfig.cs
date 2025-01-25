using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Jwa;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Identidade.API.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddJwksManager(options => options.Jws = Algorithm.Create(DigitalSignaturesAlgorithm.EcdsaSha256))//add jwksManeger e definindo esquema de criptografia
                .PersistKeysToDatabaseStore<ApplicationDbContext>()
                .UseJwtValidation();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                            .AddRoles<IdentityRole>()
                            .AddErrorDescriber<IdentityMensagensPortugues>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();//token para caso precise resetar uma senha, autenticar uma conta recem gerada. Em outras palavras, uma criptografia dentro de um link para te reconhecer

            //Não precisa mais estar aqui (config pra entender o token) uma vez que apos a implementação do JWK essa api não precisa mais do AppSettings com infos do Token, a não ser que coloque outras funcionalidade aqui que dependa de um usuário autenticado. NEcessário colocar section AppSettings com Url da chave privada caso for usar.
            //services.AddJwtConfiguration(configuration);

            return services;
        }

    }
}