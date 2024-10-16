using FluentValidation;
using FluentValidation.Results;

namespace NSE.Carrinho.API.Model
{
    public class CarrinhoCliente //carrinho do cliente
    {
        /*
                Lembrando que optamos por uma arquitetura super-simples propositalmente, nosso Models não são DTOs, 
                são como se fossem entidades, mas não herdam de Entity.cs e tudo mais, pois optamos por essa simplicidade. 
                Temos regra dee negócio e tudo mais, porém são modelos simplificados(não tendem a querer implementar todas 
                as práticas do DDD, Modelos Ricos e tudo mais)
         */

        internal const int MAX_QUANTIDADE_ITEM = 5;

        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public List<CarrinhoItem> Itens { get; set; } = new List<CarrinhoItem>();
        public ValidationResult ValidationResult { get; set; }

        public bool VoucherUtilizado { get; set; }
        public decimal Desconto { get; set; }

        public Voucher Voucher { get; set; }


        public CarrinhoCliente(Guid clienteId)
        {
            Id = Guid.NewGuid();
            ClienteId = clienteId;
        }

        public CarrinhoCliente() { }

        public void AplicarVoucher(Voucher voucher)
        {
            Voucher = voucher;
            VoucherUtilizado = true;
            CalcularValorCarrinho();
        }

        internal void CalcularValorCarrinho()
        {
            ValorTotal = Itens.Sum(p => p.CalcularValor());
        }

        private void CalcularValorTotalDesconto()
        {
            if (!VoucherUtilizado) return;

            decimal desconto = 0;
            var valor = ValorTotal;

            if (Voucher.TipoDesconto == TipoDescontoVoucher.Porcentagem)
            {
                if (Voucher.Percentual.HasValue)
                {
                    desconto = (valor * Voucher.Percentual.Value) / 100;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }

            ValorTotal = valor < 0 ? 0 : valor; //se o ValorTotal ficar menor que 0, fica sendo 0, senão o valor restante
            Desconto = desconto; //seto o desconto
        }

        internal bool CarrinhoItemExistente(CarrinhoItem item)
        {
            return Itens.Any(p => p.ProdutoId == item.ProdutoId);
        }

        internal CarrinhoItem ObterPorProdutoId(Guid produtoId)
        {
            return Itens.FirstOrDefault(p => p.ProdutoId == produtoId);
        }

        internal void AdicionarItem(CarrinhoItem item)
        {
            item.AssociarCarrinho(Id);

            if (CarrinhoItemExistente(item))
            {
                var itemExistente = ObterPorProdutoId(item.ProdutoId);
                itemExistente.AdicionarUnidades(item.Quantidade);

                item = itemExistente;
                Itens.Remove(itemExistente);
            }

            Itens.Add(item);
            CalcularValorCarrinho();
        }

        internal void AtualizarItem(CarrinhoItem item)//remover item com qtd antiga e adicioonar com qtd nova
        {
            item.AssociarCarrinho(Id);

            var itemExistente = ObterPorProdutoId(item.ProdutoId);

            Itens.Remove(itemExistente);
            Itens.Add(item);

            CalcularValorCarrinho();
        }

        internal void AtualizarUnidades(CarrinhoItem item, int unidades)//att unidades do item recebido
        {
            item.AtualizarUnidades(unidades); //att unidades do item
            AtualizarItem(item); //remove item existente e adiciona item att
        }

        internal void RemoverItem(CarrinhoItem item)
        {
            Itens.Remove(ObterPorProdutoId(item.ProdutoId));
            CalcularValorCarrinho();
        }

        internal bool EhValido() //validar todos os itens do carrinho e tb o próprio carrinho
        {
            var erros = Itens.SelectMany(i => new CarrinhoItem.ItemCarrinhoValidation().Validate(i).Errors).ToList(); //para cada item do meu carrinho vou executar a validação ItemCarrinhoValidation e retornar uma coleção de erros que vou materializar para uma lista
            erros.AddRange(new CarrinhoClienteValidation().Validate(this).Errors); //adicionando nessa lista os erros de validação do CarrinhoClienteValidation
            ValidationResult = new ValidationResult(erros); //adicionando todos os erros no meu validationResult

            return ValidationResult.IsValid;
        }

        public class CarrinhoClienteValidation : AbstractValidator<CarrinhoCliente>
        {
            public CarrinhoClienteValidation()
            {
                RuleFor(c => c.ClienteId)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Cliente não reconhecido");

                RuleFor(c => c.Itens.Count)
                    .GreaterThan(0)
                    .WithMessage("O carrinho não possui itens");

                RuleFor(c => c.ValorTotal)
                    .GreaterThan(0)
                    .WithMessage("O valor total do carrinho precisa ser maior que 0");
            }
        }
    }
}
