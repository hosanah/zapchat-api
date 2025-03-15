using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : MainController
    {
        private readonly IAuthService _authService;

        public AuthController(INotificator notificator, IAuthService authService) : base(notificator)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<dynamic>> Login(LoginUserRequestDto userRequest)
        {
            return CustomResponse(await _authService.LoginUserAutenticate(userRequest));
        }
    }
}
