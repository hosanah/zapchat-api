using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.Messages;
using Zapchat.Service.Services;

namespace Zapchat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : MainController
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService, INotificator notificator) : base(notificator)
        {
            _usuarioService = usuarioService;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioDto usuarioDto)
        {
            return CustomResponse(await _usuarioService.AddAsync(usuarioDto));
        }
        
    }
}
