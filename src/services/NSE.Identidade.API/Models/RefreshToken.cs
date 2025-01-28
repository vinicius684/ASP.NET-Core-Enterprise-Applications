using System;

namespace NSE.Identidade.API.Models
{
    public class RefreshToken
    {
        public RefreshToken()
        {
            Id = Guid.NewGuid();
            Token = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Username { get; set; } //email ou id
        public Guid Token { get; set; } //não é como se fosse um JWT com um tanto de dados serializados, só precisa de ser um dado único
        public DateTime ExpirationDate { get; set; }
    }
}