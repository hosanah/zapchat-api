using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Domain.Notifications
{
    public class Notificator : INotificator
    {
        private readonly List<Notification> _notification;
        public Notificator()
        {
            _notification = new List<Notification>();
        }

        public List<Notification> GetNotifications()
        {
            return _notification;
        }

        public void Handle(Notification notification)
        {
            _notification.Add(notification);
        }

        public bool HasNotification()
        {
            return _notification.Any();
        }

    }
}
