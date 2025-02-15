using Microsoft.AspNetCore.Mvc;
using Zapchat.Domain.Interfaces.Messages;
using Zapchat.Domain.Notifications;

namespace Zapchat.Api.Controllers
{
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly INotificator _notificator;
        protected MainController(INotificator notificator)
        {
            _notificator = notificator;
        }

        protected ActionResult CustomResponse(object result = null!)
        {
            if (OperacaoValida())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notificator.GetNotifications().Select(n => n.Message)
            });
        }

        protected bool OperacaoValida()
        {
            return !_notificator.HasNotification();
        }

        protected void NotificarErro(string message)
        {
            _notificator.Handle(new Notification(message));
        }
    }
}
