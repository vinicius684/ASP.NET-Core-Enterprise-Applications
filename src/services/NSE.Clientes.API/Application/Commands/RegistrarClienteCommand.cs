using FluentValidation;
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

        public override bool EhValido()
        {
            ValidationResult = new RegistrarClienteValidation().Validate(this);
            return ValidationResult.IsValid;
        }


        public class RegistrarClienteValidation : AbstractValidator<RegistrarClienteCommand> //classe aninhada, classe dentro de outra classe
        {
            public RegistrarClienteValidation()
            {
                RuleFor(c => c.Id)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Id do cliente inválido");

                RuleFor(c => c.Nome)
                    .NotEmpty()
                    .WithMessage("O nome do cliente não foi informado");

                RuleFor(c => c.Cpf)
                    .Must(TerCpfValido)
                    .WithMessage("O CPF informado não é válido.");

                RuleFor(c => c.Email)
                    .Must(TerEmailValido)
                    .WithMessage("O e-mail informado não é válido.");
            }

            protected static bool TerCpfValido(string cpf)
            {
                return Core.DomainObjects.Cpf.Validar(cpf);
            }

            protected static bool TerEmailValido(string email)
            {
                return Core.DomainObjects.Email.Validar(email);
            }
        }
    }
}
