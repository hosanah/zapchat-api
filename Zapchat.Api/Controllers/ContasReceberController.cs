using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.DTOs.ContasReceber;
using Zapchat.Domain.Interfaces.ContasReceber;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContasReceberController : MainController
    {
        private readonly IContasReceberService _contasReceberService;

        public ContasReceberController(INotificator notificator, IContasReceberService contasReceberService) : base(notificator)
        {
            _contasReceberService = contasReceberService;
        }

        [HttpPost]
        public async Task<ActionResult> ListarContasReceberExcel([FromBody] ListarContasReceberExcelDto listarContaDto)
        {
            return CustomResponse(await _contasReceberService.ListarContasReceberExcel(listarContaDto));
        }
    }
}
