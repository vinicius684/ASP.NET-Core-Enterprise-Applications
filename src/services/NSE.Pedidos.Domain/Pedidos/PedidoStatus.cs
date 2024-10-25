namespace NSE.Pedidos.Domain.Pedidos
{
    public enum PedidoStatus
    {
        Autorizado = 1, //pagamento autorizado mas não capturado
        Pago = 2,
        Recusado = 3,
        Entregue = 4,
        Cancelado = 5
    }
}