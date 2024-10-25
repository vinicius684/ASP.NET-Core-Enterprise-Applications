namespace NSE.Pedidos.Domain.Pedidos
{
    /*
        Apesar de Endereço pertencer a Cliente, mudando de contexto as entidades possuem significados diferentes.
        Nesse BC, endereço não é nem uma entidade, nem raiz de agregação. É apenas um objeto de valor que vai ser persistido como dado de Pedido

        Se o endereço do Cliente mudar. Preciso de ter o endereço do Pedido ainda
     */
    public class Endereco
    {
        /*
            Get e set aberto mesmo. O que me importa aqui é a estrutura dos dados para ser usada na tabela Pedido
         */
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
}