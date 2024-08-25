using System.Text;
using NSE.Identidade.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using NSE.Identidade.API.Extensions;


//Configura os servi�os que a aplica��o vai usar, registrando-os no container de DI.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();//token para caso precise resetar uma senha, autenticar uma conta recem gerada. Em outras palavras, uma criptografia dentro de um link para te reconhecer

// JWT
var configuration = builder.Configuration;

var appSettingsSection = configuration.GetSection("AppSettings");//atraves da classe configuration vou obter uma se��o do app settings
builder.Services.Configure<AppSettings>(appSettingsSection);//"pedindo" classe AppSettings represente os dados da se��o obtida

var appSettings = appSettingsSection.Get<AppSettings>();//atrav�s da section, obtendo a classe j� populada
var key = Encoding.ASCII.GetBytes(appSettings.Secret);//tranformando minha chave em uma sequencia de bytes no formato ASCII

builder.Services.AddAuthentication(options =>//Dizendo que tanto a forma de autenticar, quanto o "desafio" de como apresentar e credenciar um usu�rio � feito internamente dependem do Padr�o JWT, poderia usar via cookie, via outros providers...
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(bearerOptions =>//add suporte  pra esse tipo espec�fico de token e op��es
{
    bearerOptions.RequireHttpsMetadata = true;
    bearerOptions.SaveToken = true;
    bearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,//validar o emissor com base na assinatura, n�o posso utilizar um token qualquer com uma ssinatura qualquer
        IssuerSigningKey = new SymmetricSecurityKey(key),//assinatura do emissor, segredo do token
        ValidateIssuer = true,//validar emissor
        ValidateAudience = true,//validar onde o oken � v�lido
        //ValidAudiences =
        ValidAudience = appSettings.ValidoEm,
        ValidIssuer = appSettings.Emissor,
    };
}); 
//

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

//Configura o pipeline de requisi��es, adicionando middlewares que definem como cada requisi��o ser� tratada.
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
