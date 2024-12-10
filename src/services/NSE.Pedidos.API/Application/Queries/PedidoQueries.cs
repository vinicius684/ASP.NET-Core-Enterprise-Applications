using Dapper;
using NSE.Pedidos.API.Application.DTO;
using NSE.Pedidos.Domain.Pedidos;

namespace NSE.Pedidos.API.Application.Queries
{
    public interface IPedidoQueries
    {
        Task<PedidoDTO> ObterUltimoPedido(Guid clienteId); //usado simplesmente quando você precisar finalizar o pedido
        Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(Guid clienteId);
        Task<PedidoDTO> ObterPedidosAutorizados();

    }

    public class PedidoQueries : IPedidoQueries
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoQueries(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<PedidoDTO> ObterUltimoPedido(Guid clienteId)
        {
            /*
               Um motivo usu real do dapper é quando sua app não requer o uso de um ferramental mais completo como o EF ou as vezes apenas por preferencia mesmo

              Tb poderia utilizar o próprio repository, criar um método especifico do Ef pra isso... mas aqui vamos utilizar o Dapper.

              Edu está utilizando Dapper de propósito nesse caso pra mostrar que as classes de Queries tem "propriedade" pra acessar 
              o banco(fazer consultas personalizadas), mesmo sendo na camada de application
             
              Enquanto o repository serve pra métodos embutidos para trabalhar com o negócio, as Queries vão trazer outras leituras pra 
              atender as necessidades de exibir dados de forma diferente da qual você manipula ele dentro do seu domínio.
             */


            /*
              Selecionando todos os campos que eu preciso apenas, de pedidos, fazendo Inner Join com pedidoItens, em que os Ids são do mesmo pedido, onde Pedido.ClienteId = clienteId AND a DataCadastro esteja entre 3 minutos atras e agora AND Pedido.PedidoStatus = 1, ordenando em Pedido.DataCadastro decrescente
            
              As consultas podem ser colocadas em arquivos de Resouce(aqueles utilizados para tradução)

              Não concatene a variável clinteId com sql, estaria dando um becha de sql injection. Deve ser passado como parâmetro  na hora de declarar a consulta

              Poderia abrir uma sql connection aqui, passando a connectionString, mas Edu acha trabalhoso se já temos tudo na mão, ele costuma pega emprestado a conexão sql do próprio Entityframework

              A consulta poderia ser uma view ou procedure chamada do banco, unico problema é que se a view ou procedure mudar, sua app quebra

              Daria ainda pra fazer 3 selects, mapeando entidade por entidade e compondo minha PedidoDTO...
             */

            const string sql = @"SELECT
                                P.ID AS 'ProdutoId', P.CODIGO, P.VOUCHERUTILIZADO, P.DESCONTO, P.VALORTOTAL,P.PEDIDOSTATUS,
                                P.LOGRADOURO,P.NUMERO, P.BAIRRO, P.CEP, P.COMPLEMENTO, P.CIDADE, P.ESTADO,
                                PIT.ID AS 'ProdutoItemId',PIT.PRODUTONOME, PIT.QUANTIDADE, PIT.PRODUTOIMAGEM, PIT.VALORUNITARIO 
                                FROM PEDIDOS P 
                                INNER JOIN PEDIDOITEMS PIT ON P.ID = PIT.PEDIDOID 
                                WHERE P.CLIENTEID = @clienteId 
                                AND P.DATACADASTRO between DATEADD(second, -30,  GETDATE()) and DATEADD(minute, 0,  GETDATE())
                                AND P.PEDIDOSTATUS = 1 
                                ORDER BY P.DATACADASTRO DESC";

            //Poderia ser resolvido assim, mas não vai dar cero pois Endereco não é tabela, o dapper não vai saber mapear
            //var pedido = await _pedidoRepository.ObterConexao()
            //    .QueryAsync<PedidoDTO, PedidoItemDTO, EnderecoDTO, PedidoDTO>(sql, (p, pi, e) => 
            //    {
            //        p.PedidoItems.Add(pi);
            //        p.Endereco = e;
            //        return p;
            //    }, new { clienteId }, splitOn:"ProdutoId, ProdutoItemId");

            var pedido = await _pedidoRepository.ObterConexao()
                .QueryAsync<dynamic>(sql, new { clienteId });

            return MapearPedido(pedido); //Select - percorre cada elemento da coleção original e aplica uma função de tranformação
        }

        public async Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(Guid clienteId)
        {
            var pedidos = await _pedidoRepository.ObterListaPorClienteId(clienteId);

            return pedidos.Select(PedidoDTO.ParaPedidoDTO);
        }

        public async Task<PedidoDTO> ObterPedidosAutorizados()
        {
            // Correção para pegar todos os itens do pedido e ordernar pelo pedido mais antigo
            /*
              Ponto interessante a comentar: seleciona o Id do Pedido e da um álias, mas tb seleciona o Id do pedido sem dar álias nenhum. A mesma coisa com id do PedidoItem.
              Pois está selecionando duas tabelas(Join), isso é necessário pq nos próximos passos fala pro dapper que vai receber um PedidoDTO e um PedidoItemDTO, só que no final vai devolver um PedidoDTO. 
              Em outras palavras preciso popular os Ids, o alias não faz isso, ele é basicamente pra auxliar o dapper
             
             */
            const string sql = @"SELECT 
                                P.ID as 'PedidoId', P.ID, P.CLIENTEID, 
                                PI.ID as 'PedidoItemId', PI.ID, PI.PRODUTOID, PI.QUANTIDADE 
                                FROM PEDIDOS P 
                                INNER JOIN PEDIDOITEMS PI ON P.ID = PI.PEDIDOID 
                                WHERE P.PEDIDOSTATUS = 1                                
                                ORDER BY P.DATACADASTRO";

            var pedido = await _pedidoRepository.ObterConexao().QueryAsync<PedidoDTO, PedidoItemDTO, PedidoDTO>(sql,
                 (p, pi) =>
                 {                    
                     p.PedidoItems ??= new List<PedidoItemDTO>();
                     p.PedidoItems.Add(pi);

                     return p;
                 }, splitOn: "PedidoId,PedidoItemId");

            return pedido.FirstOrDefault();
        }

        private PedidoDTO MapearPedido(dynamic result)
        {
            var pedido = new PedidoDTO//o que precisa obter de pedido vai estar na primeira linha e repedido nas outras, ou seja só quero pegar da primeira linha
            {
                Codigo = result[0].CODIGO,
                Status = result[0].PEDIDOSTATUS,
                ValorTotal = result[0].VALORTOTAL,
                Desconto = result[0].DESCONTO,
                VoucherUtilizado = result[0].VOUCHERUTILIZADO,

                PedidoItems = new List<PedidoItemDTO>(),
                Endereco = new EnderecoDTO
                {
                    Logradouro = result[0].LOGRADOURO,
                    Bairro = result[0].BAIRRO,
                    Cep = result[0].CEP,
                    Cidade = result[0].CIDADE,
                    Complemento = result[0].COMPLEMENTO,
                    Estado = result[0].ESTADO,
                    Numero = result[0].NUMERO
                }
            };

            foreach (var item in result)//porque os demais itens de pedido tb estão misturados com Pedido, Endereço... Portanto pegando apenas os dados de pedido
            {
                var pedidoItem = new PedidoItemDTO
                {
                    Nome = item.PRODUTONOME,
                    Valor = item.VALORUNITARIO,
                    Quantidade = item.QUANTIDADE,
                    Imagem = item.PRODUTOIMAGEM
                };

                pedido.PedidoItems.Add(pedidoItem);
            }

            return pedido;
        }
    }
}
