using System.Threading.Tasks;
using NSE.Pagamentos.API.Models;

namespace NSE.Pagamentos.Facade
{
    public interface IPagamentoFacade
    {
        Task<Transacao> AutorizarPagamento(Pagamento pagamento);//realizar a transação - Passo um pagamento e espero receber uma transação
        //Task<Transacao> CapturarPagamento(Transacao transacao);
        //Task<Transacao> CancelarAutorizacao(Transacao transacao);
    }
}