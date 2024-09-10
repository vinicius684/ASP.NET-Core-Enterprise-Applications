namespace NSE.Core.Messages.Integration
{ 

    public class UsuarioRegistradoIntegrationEvent : IntegrationEvent //usuario registrado, vai se tornar um cliente agora. Não está sendo colocada na api de autenticação porque ela tb pode ser usada por Cliente
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Cpf { get; private set; }

        public UsuarioRegistradoIntegrationEvent(Guid id, string nome, string email, string cpf)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Cpf = cpf;
        }
    }
}
