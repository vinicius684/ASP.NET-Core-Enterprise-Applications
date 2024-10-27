namespace NSE.Pedidos.API.Application.DTO
{
    public class EnderecoDTO
    {
        //Mesmas propriedades da entidade Endereco desse BC, porém foi duplicado pois é uma visão, uma responsabilidade diferente nesse cenário
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
}