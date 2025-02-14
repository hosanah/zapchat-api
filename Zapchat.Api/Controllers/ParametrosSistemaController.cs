﻿using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.Interfaces;

namespace Zapchat.Api.Controllers
{
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
