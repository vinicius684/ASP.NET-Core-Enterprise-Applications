using NSE.Core.DomainObjects;

namespace NSE.Catalogo.API.Models
{
    public class Produto : Entity, IAggregateRoot 
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Imagem { get; set; } //poderia possuir mais de uma
        public int QuantidadeEstoque { get; set; } //por ser uma app mais enxuta, Edu não implementaria uma outra API de estoque. Mas se for uma app mais complexa, criaria uma api Estoque e fazeria a integração entre as apis através de fila
    }
}
