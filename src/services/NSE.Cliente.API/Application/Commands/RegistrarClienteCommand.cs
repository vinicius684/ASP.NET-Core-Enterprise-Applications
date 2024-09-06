using NSE.Clientes.API.Models;
using NSE.Core.DomainObjects;
using NSE.Core.Messages;

namespace NSE.Clientes.API.Application.Commands
{
    public class RegistrarClienteCommand : Command //no nosso negócio, quando estiver criando o cliente através do cadastro não vou informar o endereço
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; } 
        public string Cpf { get; private set; }

        public RegistrarClienteCommand(Guid id, string nome, string email, string cpf)
        {
            AggregateId = id;
            Id = id;
            Nome = nome;
            Email = email;
            Cpf = cpf;
        }
    }
}
