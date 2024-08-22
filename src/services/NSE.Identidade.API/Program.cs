using Microsoft.Extensions.Configuration;
using NSE.Identidade.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

//Configura os serviços que a aplicação vai usar, registrando-os no container de DI.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                //.AddErrorDescriber<IdentityMensagensPortugues>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();// token para caso precise resetar ums enha, autenticar uma conta recem gerada. Em outras palavras, um criptografia dentro de um link para te reconhecer

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configura o pipeline de requisições, adicionando middlewares que definem como cada requisição será tratada.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();//utilize esquema de rotas

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
