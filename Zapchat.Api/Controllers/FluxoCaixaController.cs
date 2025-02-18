using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.DTOs.FluxoCaixa;
using Zapchat.Domain.Interfaces.FluxoCaixa;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FluxoCaixaController : MainController
    {
        private readonly IFluxoCaixaService _fluxoCaixaService;

        public FluxoCaixaController(INotificator notificator, IFluxoCaixaService fluxoCaixaService)
            : base(notificator)
        {
            _fluxoCaixaService = fluxoCaixaService;
        }

        /// <summary>
        /// Consulta as contas correntes, obtém extrato de cada uma e gera relatório em Excel (Base64).
        /// </summary>
        [HttpPost("FluxoDeCaixa")]
        public async Task<ActionResult> ListarFluxoCaixaExcel([FromBody] ListarFluxoCaixaDto fluxoCaixaDto)
        {
            var resultado = await _fluxoCaixaService.ListarFluxoCaixaExcel(fluxoCaixaDto);
            return CustomResponse(resultado);
        }
    }
}
