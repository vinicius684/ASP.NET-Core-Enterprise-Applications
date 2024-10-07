namespace NSE.Bff.Compras.Models
{
    public class ItemProdutoDTO //representação Item Produto Catalogo
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Imagem { get; set; }
        public int QuantidadeEstoque { get; set; }
    }
}
