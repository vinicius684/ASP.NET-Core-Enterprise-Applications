using MediatR;

namespace NSE.Clientes.API.Application.Events
{
    public class ClienteEventHandler : INotificationHandler<ClienteRegistradoEvent>
    {
        public Task Handle(ClienteRegistradoEvent notification, CancellationToken cancellationToken)
        {
            /*
               Até o momento não está vendo a necessidade de implementação.
               
               Porém, Edu deixa como sugestão enviar um Enviar evento de confirmação. 
               Seja bem vindo cliente tal, blabla bla. Boas Compras (Email). 
               Claro config do emil n vai ser aqui, vai ter uma classe de infra que vai enviar e será injetada aqui. 
            */

            return Task.CompletedTask;
        }
    }
}
