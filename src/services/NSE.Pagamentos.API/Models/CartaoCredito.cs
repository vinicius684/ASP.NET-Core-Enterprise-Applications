namespace NSE.Pagamentos.API.Models
{
    /*
        Não deve ser persistido no banco, por questão de segurança, não deve salvar dados de cartão no banco. A não ser que isso seja necessário, mas aí vai usar segurança, criptografia e tudo mais
     */
    public class CartaoCredito
    {
        public string NomeCartao { get; set; }
        public string NumeroCartao { get; set; }
        public string MesAnoVencimento { get; set; }
        public string CVV { get; set; }

        protected CartaoCredito() { }

        public CartaoCredito(string nomeCartao, string numeroCartao, string mesAnoVencimento, string cvv)
        {
            NomeCartao = nomeCartao;
            NumeroCartao = numeroCartao;
            MesAnoVencimento = mesAnoVencimento;
            CVV = cvv;
        }
    }
}