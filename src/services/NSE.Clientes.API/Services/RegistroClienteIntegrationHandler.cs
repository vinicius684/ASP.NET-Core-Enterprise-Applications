using FluentValidation.Results;
using NSE.Clientes.API.Application.Commands;
using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;



namespace NSE.Clientes.API.Services
{
    /*
         O BackgroundService no .NET Core é uma feature usada para criar workers que rodam em segundo plano, 
        de forma independente do ciclo de vida de requisições HTTP no ASP.NET. Embora seja registrado no 
        pipeline da aplicação, ele processa tarefas de forma contínua e paralela, sem depender de um request específico.
     */

    public class RegistroClienteIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RegistroClienteIntegrationHandler(
                            IServiceProvider serviceProvider, 
                            IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        private void SetResponder() 
        {

            _bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request => //esperar por uma classe UsuarioRegistradoIntegrationEvent e responder um ResponseMessage
               await RegistrarCliente(request));

            _bus.AdvancedBus.Connected += OnConnect; //evento - quando o bus-advancedbus estiver connectado - reativa as assinaturas para eventos após uma reconexão
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetResponder();

            return Task.CompletedTask;
        }

        private void OnConnect(object s, EventArgs e)
        {
            SetResponder(); //background só inicializa uma vez, logo se a coneção é perdida, ela não é exatamente recriada, logo recriando o bus e sua assinatura
        }

        private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistradoIntegrationEvent message)
        {
            var clienteCommand = new RegistrarClienteCommand(message.Id, message.Nome, message.Email, message.Cpf);
            ValidationResult sucesso;

            using (var scope = _serviceProvider.CreateScope())//ServiceLocator - Criando uma instancia de um serviceScoped. Utilizado para resolver serviços dentro de serviços com life cicle Scoped
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
                sucesso = await mediator.EnviarComando(clienteCommand);
            }

            return new ResponseMessage(sucesso);
        }
    }
}
