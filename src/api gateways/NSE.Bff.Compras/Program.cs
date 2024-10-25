
using Microsoft.Extensions.Configuration;
using NSE.Bff.Compras.Configuration;
using NSE.WebAPI.Core.Identidade;



var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddApiConfiguration(builder.Configuration);//

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();//

builder.Services.RegisterServices();

builder.Services.AddMessageBusConfiguration(builder.Configuration);

//Configura o pipeline de requisi��es, adicionando middlewares que definem como cada requisi��o ser� tratada.
var app = builder.Build();

app.UseSwaggerConfiguration();

app.UseApiConfiguration(builder.Environment);

app.Run();