using System;
using System.Collections.Generic;

namespace NSE.WebApp.MVC.Models
{
    public class CarrinhoViewModel 
    {
        //ClienteId e CarrinhoId irrelevantes nesse caso. ClienteId obtido pelo token e CarrinhoId só precisa ser conhecido pela própria api que vai manipulá-lo utilizando o ClienteId
        public decimal ValorTotal { get; set; }
        public VoucherViewModel Voucher { get; set; }
        public bool VoucherUtilizado { get; set; }
        public decimal Desconto { get; set; }
        public List<ItemCarrinhoViewModel> Itens { get; set; } = new List<ItemCarrinhoViewModel>();
    }

    public class ItemCarrinhoViewModel
    {
        //ItemId irrelevante nesse caso. Só faz sentido para a api que irá manipulá-lo utilizando o ProdutoId
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string Imagem { get; set; }
    }
}