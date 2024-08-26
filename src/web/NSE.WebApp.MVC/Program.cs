using NSE.WebApp.MVC.Configuration;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityConfiguration();//

builder.Services.AddMvcConfiguration();//






var app = builder.Build();

app.UseMvcConfiguration(app.Environment);//

app.Run();
