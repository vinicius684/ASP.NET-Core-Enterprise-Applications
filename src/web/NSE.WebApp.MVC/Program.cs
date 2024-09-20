using NSE.Core.Utils;
using NSE.WebApp.MVC.Configuration;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddIdentityConfiguration();//

builder.Services.AddMvcConfiguration(builder.Configuration);//

builder.Services.RegisterServices(builder.Configuration);//


string cpfComFormatacao = "733.970.050-19";
string cpfSemFormatacao = cpfComFormatacao.ApenasNumeros(cpfComFormatacao);
Console.WriteLine(cpfSemFormatacao); // Deve exibir "73397005019"

var app = builder.Build();

app.UseMvcConfiguration(app.Environment);//

app.Run();
