using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Extensions
{
    public class PaginacaoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IPagedList modeloPaginado) //só invoke, pois não vai chamar nenhum método assincrono. Tb preciso receber um modelo
        {
            return View(modeloPaginado);
        }
    }
}