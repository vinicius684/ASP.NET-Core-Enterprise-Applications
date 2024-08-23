using Microsoft.Extensions.Configuration;
using NSE.Identidade.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

//Configura os serviços que a aplicação vai usar, registrando-os no container de DI.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                //.AddErrorDescriber<IdentityMensagensPortugues>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();//token para caso precise resetar uma senha, autenticar uma conta recem gerada. Em outras palavras, uma criptografia dentro de um link para te reconhecer

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NerdStore Enterprise Identity API",
        Description = "Esta API faz parte do curso ASP.NET Core Enterprise Applications.",
        Contact = new OpenApiContact() { Name = "Vinicius P", Email = "vpinholi6@gmail.com" },
        License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
    });
});

//Configura o pipeline de requisições, adicionando middlewares que definem como cada requisição será tratada.
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();//utilize esquema de rotas

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
