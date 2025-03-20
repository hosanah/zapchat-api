using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Interfaces.Messages;
using Microsoft.AspNetCore.Authorization;
using Zapchat.Domain.Interfaces.ContaAazul;

namespace Zapchat.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GrupoWhatsAppController : MainController
    {
        private readonly IGrupoWhatsAppService _grupoWhatsAppservice;

        public GrupoWhatsAppController(INotificator notificator, IGrupoWhatsAppService grupoWhatsAppservice) : base(notificator)
        {
            _grupoWhatsAppservice = grupoWhatsAppservice;
        }

        [HttpPost("Configurar")]
        public async Task<IActionResult> AutoConfigurarGrupo([FromBody] AutoConfigurarGrupoDto dto)
        {
            return CustomResponse(await _grupoWhatsAppservice.AutoConfigurarGrupo(dto));
        }


        [HttpGet("BuscarTodasConfiguracoes")]
        public async Task<IEnumerable<AutoConfigurarGrupoDto>> ListarTodos() => await _grupoWhatsAppservice.BuscarTodasConfigurações();


        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _grupoWhatsAppservice.DeleteAsync(id);
            return NoContent();
        }

        /*[HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var grupo = await _grupoWhatsAppservice.GetByIdAsync(id);
            if (grupo == null)
                return NotFound();

            return Ok(grupo);
        }

        
        [HttpGet]
        public async Task<IEnumerable<GrupoWhatsAppDto>> ListarTodos() => await _grupoWhatsAppservice.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var grupo = await _grupoWhatsAppservice.GetByIdAsync(id);
            if (grupo == null)
                return NotFound();

            return Ok(grupo);
        }
        


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GrupoWhatsAppDto dto)
        {
            try
            {
                var grupoWhats = await _grupoWhatsAppservice.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = grupoWhats.Id }, grupoWhats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] GrupoWhatsAppDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _grupoWhatsAppservice.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _grupoWhatsAppservice.DeleteAsync(id);
            return NoContent();
        }*/
    }
}
