using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Pedidos.API.Application.DTO;
using NSE.Pedidos.API.Application.Queries;
using NSE.WebAPI.Core.Controllers;
using System.Net;

namespace NSE.Pedidos.API.Controllers
{
    [Authorize]
    public class VoucherController : MainController
    {
        private readonly IVoucherQueries _voucherQueries;

        public VoucherController(IVoucherQueries voucherQueries)
        {
            _voucherQueries = voucherQueries;
        }

        [HttpGet("voucher/{codigo}")]
        [ProducesResponseType(typeof(VoucherDTO), (int)HttpStatusCode.OK)] //Vou produzir uma reposta do Tipo VoucherDTO retornando um OK
        [ProducesResponseType((int)HttpStatusCode.NotFound)] //Ou um NotFound. è interessante pois a documentação vai expressar essas infos de forma mas clara pra quem for consumir
        public async Task<IActionResult> ObterPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return NotFound(); //Codigo vazio NotFound(recurso não encontrado)

            var voucher = await _voucherQueries.ObterVoucherPorCodigo(codigo);

            return voucher == null ? NotFound() : CustomResponse(voucher); //Se o Voucher for nulo, retorno um NotFound.  
        }
    }
}
