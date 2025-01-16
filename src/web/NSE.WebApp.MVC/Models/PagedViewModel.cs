using System;
using System.Collections.Generic;

namespace NSE.WebApp.MVC.Models
{
    public class PagedViewModel<T> where T : class
    {
        public string ReferenceAction { get; set; }
        public IEnumerable<T> List { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Query { get; set; }
        public int TotalResults { get; set; }
        public double TotalPages => Math.Ceiling((double)TotalResults / PageSize); //Math.Ceiling - se o numero tiver parte decimal arredonda pra cima, se for inteiro permanece inalterado
    }
    //public interface IPagedList
    //{
    //    public string ReferenceAction { get; set; }
    //    public int PageIndex { get; set; }
    //    public int PageSize { get; set; }
    //    public string Query { get; set; }
    //    public int TotalResults { get; set; }
    //    public double TotalPages { get; }
    //}
}