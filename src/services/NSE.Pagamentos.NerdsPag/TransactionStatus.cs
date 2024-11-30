namespace NSE.Pagamentos.NerdsPag
{
    public enum TransactionStatus
    {
        Authorized = 1, //valor reservado no cartão do cliente
        Paid, //capturado
        Refused,
        Chargedback,//devolve o dinheiro após uma compra
        Cancelled
    }
}