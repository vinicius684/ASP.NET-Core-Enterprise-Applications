namespace NSE.Carrinho.API.Model
{
    public class Voucher
    {
        public decimal? Percentual { get; set; }
        public decimal? ValorDesconto { get; set; }
        public string Codigo { get; set; }
        public TipoDescontoVoucher TipoDesconto { get; set; }
    }

    public enum TipoDescontoVoucher //aqui em API.Carrinho estamos utilizando Enum pois vamos precisar pra validar regra de negócio. Velhor que ficar comparando 1 e 0 é usar um Enum
    {
        Porcentagem = 0,
        Valor = 1
    }

}
