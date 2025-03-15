using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.Interfaces;

namespace Zapchat.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosSistemaController : ControllerBase
    {
        private readonly IParametroSistemaService _service;

        public ParametrosSistemaController(IParametroSistemaService service)
        {
            _service = service;
        }

    }
}
