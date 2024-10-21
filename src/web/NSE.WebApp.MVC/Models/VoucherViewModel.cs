namespace NSE.WebApp.MVC.Models
{
    public class VoucherViewModel
    {
        /*
            Extremamente simples, apenas o código. É importante ter esse tipo pois na hr de desserializar, lá na api de carrinho e bff compras o Carrinho tem um nó Voucher
         */
        public string Codigo { get; set; }
    }
}