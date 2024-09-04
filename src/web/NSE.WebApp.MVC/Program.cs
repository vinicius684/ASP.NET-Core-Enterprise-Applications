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




var app = builder.Build();

app.UseMvcConfiguration(app.Environment);//

app.Run();
