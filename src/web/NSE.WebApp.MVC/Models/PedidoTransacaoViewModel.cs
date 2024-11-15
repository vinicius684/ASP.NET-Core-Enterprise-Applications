using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace NSE.WebApp.MVC.Models
{
    public class PedidoTransacaoViewModel
    {
        #region Pedido

        public decimal ValorTotal { get; set; }
        public decimal Desconto { get; set; }
        public string VoucherCodigo { get; set; }
        public bool VoucherUtilizado { get; set; }

        public List<ItemCarrinhoViewModel> Itens { get; set; } = new List<ItemCarrinhoViewModel>();

        #endregion

        #region Endereco

        public EnderecoViewModel Endereco { get; set; }

        #endregion

    }
}