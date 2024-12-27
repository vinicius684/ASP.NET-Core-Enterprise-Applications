using System.Collections.Generic;

namespace NSE.Catalogo.API.Models
{
    public class PagedResult<T> where T : class
    {
        public IEnumerable<T> List { get; set; }//lista paginada
        public int TotalResults { get; set; }
        public int PageIndex { get; set; }//qual página está
        public int PageSize { get; set; }//quantos em quantos itens está paginando
        public string Query { get; set; }
    }
}