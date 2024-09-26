using FluentValidation;
using System.Text.Json.Serialization;

namespace NSE.Carrinho.API.Model
{
    public class CarrinhoItem //produtos dentro do carrinho
    {
        public CarrinhoItem()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string Imagem { get; set; } //nome, imagem do produto precisam estar no carrinho? Não precisa, mas é interessante, senão vai ter que ficar batendo na CAtalogo para pegar essas informações

        public Guid CarrinhoId { get; set; }

        [JsonIgnore]
        public CarrinhoCliente CarrinhoCliente { get; set; }

    }
}