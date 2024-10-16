using NSE.Core.DomainObjects;

namespace NSE.Clientes.API.Models
{
    public class Cliente : Entity, IAggregateRoot
    {
        public string Nome { get; private set; }
        public Email Email { get; private set; } //Email e cpf não devem ser representados por ´string, segundo o DDD devemos especilizar no domínio principalmente quando há validação - obs: forma diferente de mapear na mapping
        public Cpf Cpf { get; private set; }
        public bool Excluido { get; private set; }
        public Endereco Endereco { get; private set; }

        // EF Relation
        protected Cliente() { }

        public Cliente(Guid id, string nome, string email, string cpf) //endereço e se ele está excluídoo ou não não é interessante receber
        {
            Id = id;
            Nome = nome;
            Email = new Email(email);
            Cpf = new Cpf(cpf);
            Excluido = false;
        }

        public void TrocarEmail(string email)
        {
            Email = new Email(email);
        }

        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

    }

}
