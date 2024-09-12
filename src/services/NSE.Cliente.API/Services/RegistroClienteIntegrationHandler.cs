using EasyNetQ;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Internal;
using NSE.Clientes.API.Application.Commands;
using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;



namespace NSE.Clientes.API.Services
{
    /*
         O BackgroundService no .NET Core é uma feature usada para criar workers que rodam em segundo plano, 
        de forma independente do ciclo de vida de requisições HTTP no ASP.NET. Embora seja registrado no 
        pipeline da aplicação, ele processa tarefas de forma contínua e paralela, sem depender de um request específico.
     */

    public class RegistroClienteIntegrationHandler : BackgroundService
    {
        private IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RegistroClienteIntegrationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_bus = RabbitHutch.CreateBus("host=localhost:5672");

            _bus = RabbitHutch.CreateBus("host=localhost:5672",
                 serviceRegister => serviceRegister.EnableNewtonsoftJson());

            _bus.Rpc.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request => //esperar por uma classe UsuarioRegistradoIntegrationEvent e responder um ResponseMessage
                new ResponseMessage(await RegistrarCliente(request)));

            return Task.CompletedTask;
        }

        private async Task<ValidationResult> RegistrarCliente(UsuarioRegistradoIntegrationEvent message)
        {
            var clienteCommand = new RegistrarClienteCommand(message.Id, message.Nome, message.Email, message.Cpf);
            ValidationResult sucesso;

            using (var scope = _serviceProvider.CreateScope())//ServiceLocator - Criando uma instancia de um serviceScoped. Utilizado para resolver serviços dentro de serviços com life cicle Scoped
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
                sucesso = await mediator.EnviarComando(clienteCommand);
            }

            return sucesso;
        }
    }
}
