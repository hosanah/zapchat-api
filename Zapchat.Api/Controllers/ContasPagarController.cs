using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.DTOs.ContasPagar;
using Zapchat.Domain.Interfaces.ContasPagar;
using Zapchat.Domain.Interfaces.Messages;
using Zapchat.Service.Services;

namespace Zapchat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContasPagarController : MainController
    {
        private readonly IContasPagarService _contasPagarService;

        public ContasPagarController(INotificator notificator, IContasPagarService contasPagarService) : base(notificator)
        {
            _contasPagarService = contasPagarService;
        }

        [HttpPost]
        public async Task<ActionResult> ListarContasPagarExcel([FromBody] ListarContasPagarExcelDto listarContaDto)
        {
            return CustomResponse(await _contasPagarService.ListarContasPagarExcel(listarContaDto));
        }
    }
}
