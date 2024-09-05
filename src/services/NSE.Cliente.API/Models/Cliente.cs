using NSE.Core.DomainObjects;

namespace NSE.Cliente.API.Models
{
    public class Cliente : Entity, IAggregateRoot
    {
        public string Nome { get; private set; }
        public string Email { get; private set; } //Email e cpf não devem ser representados por ´string, segundo o DDD devemos especilizar no domínio principalmente quando há validação
        public string Cpf { get; private set; }
        public bool Excluido { get; private set; }
        public Endereco Endereco { get; private set; }

        // EF Relation
        protected Cliente() { }

        public Cliente(string nome, string email, string cpf) //endereço e se ele está excluídoo ou não não é interessante receber
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Excluido = false;
        }

    }

}
