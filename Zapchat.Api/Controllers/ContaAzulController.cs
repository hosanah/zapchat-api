using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.DTOs.ContaAzul;
using Zapchat.Domain.DTOs.ContasPagar;
using Zapchat.Domain.Interfaces.ContaAazul;
using Zapchat.Domain.Interfaces.ContasPagar;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContaAzulController : MainController
    {
        private readonly IContaAzulService _contaAzulService;

        public ContaAzulController(IContaAzulService contaAzulService, INotificator notificator) : base(notificator)
        {
            _contaAzulService = contaAzulService;
        }

        [HttpPost]
        public async Task<ActionResult> ListarInadiplentePorEmpresa([FromBody] CapturaInadiplentelDto inadiplentelDto)
        {
            return CustomResponse(await _contaAzulService.ListarInadiplentePorEmpresa(inadiplentelDto));
        }
    }
}
