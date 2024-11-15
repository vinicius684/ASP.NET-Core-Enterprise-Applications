using MediatR;
using FluentValidation.Results;
using NSE.Clientes.API.Models;
using NSE.Core.Messages;
using NSE.Clientes.API.Application.Events;
using NSE.Core.Mediator;

namespace NSE.Clientes.API.Application.Commands
{
    public class ClienteCommandHandler : CommandHandler, 
        IRequestHandler<RegistrarClienteCommand, ValidationResult>,
        IRequestHandler<AdicionarEnderecoCommand, ValidationResult>
    {
        private readonly IClienteRepository _clienteRepository;  

        public ClienteCommandHandler(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ValidationResult> Handle(RegistrarClienteCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;

            var cliente = new Cliente(message.Id, message.Nome, message.Email, message.Cpf);

            var clienteExistente = await _clienteRepository.ObterPorCpf(cliente.Cpf.Numero);

            if (clienteExistente != null)
            {
                AdicionarErro("Este CPF já está em uso.");
                return ValidationResult;
            }

            _clienteRepository.Adicionar(cliente);

            cliente.AdicionarEvento(new ClienteRegistradoEvent(message.Id, message.Nome, message.Email, message.Cpf));

            return await PersistirDados(_clienteRepository.UnitOfWork);//independente de salvar ou não, vai retornar o ValidationResult e quem for receber vai ter que dar um IsValid nesse VR para saber se a coisa funcionou bem ou não
        }

        public async Task<ValidationResult> Handle(AdicionarEnderecoCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;

            var endereco = new Endereco(message.Logradouro, message.Numero, message.Complemento, message.Bairro, message.Cep, message.Cidade, message.Estado, message.ClienteId);
            _clienteRepository.AdicionarEndereco(endereco);

            return await PersistirDados(_clienteRepository.UnitOfWork);
        }
    }
}
