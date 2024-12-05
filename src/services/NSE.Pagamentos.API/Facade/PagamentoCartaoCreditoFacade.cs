using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSE.Pagamentos.API.Models;
using NSE.Pagamentos.Facade;
using NSE.Pagamentos.NerdsPag;

namespace NSE.Pagamentos.CardAntiCorruption
{
    /*
        Fez com outro nome diferente da interface pra justificar que: não importa se amanhã mudar pro paypal, pag seguro... Vai implementar uma outra facade ou até mesmo modificar essa
        -> não vai precisar mexer em models, em lugar nenhum. Se tem um único ponto de manutenção é aqui nessa classe

     */
    public class PagamentoCartaoCreditoFacade : IPagamentoFacade 
    {
        private readonly PagamentoConfig _pagamentoConfig;

        public PagamentoCartaoCreditoFacade(IOptions<PagamentoConfig> pagamentoConfig)
        {
            _pagamentoConfig = pagamentoConfig.Value;
        }

        /*Implementar o meio que o meu gateway pede pra realizar minha transação, para isso é necessários eguir a documentação, como não tem documentação Edu vai explicar*/
        public async Task<Transacao> AutorizarPagamento(Pagamento pagamento)
        {
            var nerdsPagSvc = new NerdsPagService(_pagamentoConfig.DefaultApiKey,//Criando instancia do serviço
                _pagamentoConfig.DefaultEncryptionKey);

            var cardHashGen = new CardHash(nerdsPagSvc)//gerando instancia do cardHash
            {
                CardNumber = pagamento.CartaoCredito.NumeroCartao,
                CardHolderName = pagamento.CartaoCredito.NomeCartao,
                CardExpirationDate = pagamento.CartaoCredito.MesAnoVencimento,
                CardCvv = pagamento.CartaoCredito.CVV
            };
            var cardHash = cardHashGen.Generate();//devolver uma string que é aquele hash que está representando o cartão de crédido do cliente, com uma criptografia que só sua empresa tem

            var transacao = new Transaction(nerdsPagSvc)//criando instancia de transaction, passando a instancia do nerdsPagService
            {
                //o que realmente importa
                CardHash = cardHash,
                //infos pra validação dentro no gateway
                CardNumber = pagamento.CartaoCredito.NumeroCartao,
                CardHolderName = pagamento.CartaoCredito.NomeCartao,
                CardExpirationDate = pagamento.CartaoCredito.MesAnoVencimento,
                CardCvv = pagamento.CartaoCredito.CVV,

                PaymentMethod = PaymentMethod.CreditCard,
                Amount = pagamento.Valor
            };

            return ParaTransacao(await transacao.AuthorizeCardTransaction());//AuthorizeCardTransaction vai me devolver uma Transaction e quero devolver uma Transação para meu negócio. Logo necessário um método de-para
        }

        //public async Task<Transacao> CapturarPagamento(Transacao transacao)
        //{
        //    var nerdsPagSvc = new NerdsPagService(_pagamentoConfig.DefaultApiKey,
        //        _pagamentoConfig.DefaultEncryptionKey);

        //    var transaction = ParaTransaction(transacao, nerdsPagSvc);

        //    return ParaTransacao(await transaction.CaptureCardTransaction());
        //}

        //public async Task<Transacao> CancelarAutorizacao(Transacao transacao)
        //{
        //    var nerdsPagSvc = new NerdsPagService(_pagamentoConfig.DefaultApiKey,
        //        _pagamentoConfig.DefaultEncryptionKey);

        //    var transaction = ParaTransaction(transacao, nerdsPagSvc);

        //    return ParaTransacao(await transaction.CancelAuthorization());
        //}

        public static Transacao ParaTransacao(Transaction transaction)
        {
            return new Transacao
            {
                Id = Guid.NewGuid(),
                Status = (StatusTransacao) transaction.Status,
                ValorTotal = transaction.Amount,
                BandeiraCartao = transaction.CardBrand,
                CodigoAutorizacao = transaction.AuthorizationCode,
                CustoTransacao = transaction.Cost,
                DataTransacao = transaction.TransactionDate,
                NSU = transaction.Nsu,
                TID = transaction.Tid
            };
        }

        //public static Transaction ParaTransaction(Transacao transacao, NerdsPagService nerdsPagService)
        //{
        //    return new Transaction(nerdsPagService)
        //    {
        //        Status = (TransactionStatus) transacao.Status,
        //        Amount = transacao.ValorTotal,
        //        CardBrand = transacao.BandeiraCartao,
        //        AuthorizationCode = transacao.CodigoAutorizacao,
        //        Cost = transacao.CustoTransacao,
        //        Nsu = transacao.NSU,
        //        Tid = transacao.TID
        //    };
        //}
    }
}