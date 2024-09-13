using NSE.WebAPI.Core.Identidade;
using NSE.Clientes.API.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();

builder.Services.AddMediatR(a => a.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddMessageBusConfiguartion(builder.Configuration);

builder.Services.RegisterServices();

var app = builder.Build();

app.UseSwaggerConfiguration();

app.UseApiConfiguration(builder.Environment);

app.Run();
