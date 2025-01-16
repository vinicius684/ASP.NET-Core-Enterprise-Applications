using NSE.Catalogo.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NSE.Catalogo.API.Data.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly CatalogoContext _context;

        public ProdutoRepository(CatalogoContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<PagedResult<Produto>> ObterTodos(int pageSize, int pageIndex, string query = null)
        {
            /* +- como ficaria com EF
            return await _context.Produtos.AsNoTracking()
                .Skip(pageSize * (pageIndex - 1)).Take(pageSize)
               .Where(c => c.Nome.Contains(query)).ToListAsync();
            */

            /*  
               2 Se minha query for null, ignora OU senão vai filtra pelos nomes que CONTEM a query
               4 pular um número especifico de linhas(tamanho da página * (número da pagina - 1)), pois a cunsulta parte da página 0    
               5 obter/quantos registros exibir
               6-7 outra consulta que retorna o número total de itens da query (duas consultas, recurso do dapper QueryMultipleAsync)

                obs: essa query poderia ser uma store procidure sendo executada pelo dapper tb
             */
            var sql = @$"SELECT * FROM Produtos 
                        WHERE (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%') AND Ativo = 1
                        ORDER BY [Nome] 
                        OFFSET {pageSize * (pageIndex - 1)} ROWS 
                        FETCH NEXT {pageSize} ROWS ONLY;

                      SELECT COUNT(Id) 
                        FROM Produtos 
                        WHERE (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%') AND Ativo = 1;";



            var multi = await _context.Database.GetDbConnection()
                .QueryMultipleAsync(sql, new { Nome = query });

            var produtos = multi.Read<Produto>();
            var total = multi.Read<int>().FirstOrDefault();//pegar o firstOrDefault, pq sempre tras uma lista, dessa forma vai pegar só o int

            return new PagedResult<Produto>()
            {
                List = produtos,
                TotalResults = total,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Query = query
            };
        }

        public async Task<Produto> ObterPorId(Guid id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task<List<Produto>> ObterProdutosPorId(string ids)
        {
            var idsGuid = ids.Split(',')
                .Select(id => (Ok: Guid.TryParse(id, out var x), Value: x));// pra cada item, tenta converter a string id em um obj do tipo guid, retorna true se a conversão for bem-sucedida e armazena o guid em x (palvara out é o que faz isso possível)

            if (!idsGuid.All(nid => nid.Ok)) return new List<Produto>();//se alguma conversão não for bem sucedida, retorna uma lista vazia

            var idsValue = idsGuid.Select(id => id.Value);//se todos tiverem conseguido ser convertidos, vou dar um select no valor desses guids (coloca-los em uma lista)

            return await _context.Produtos.AsNoTracking()
                .Where(p => idsValue.Contains(p.Id) && p.Ativo).ToListAsync();//retorno os produtos de id que tenho pra consultarpara que  e que estejam Ativos
        }

        public void Adicionar(Produto produto)
        {
            _context.Produtos.Add(produto);
        }

        public void Atualizar(Produto produto)
        {
            _context.Produtos.Update(produto);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}