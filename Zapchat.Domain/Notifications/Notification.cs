using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapchat.Domain.Notifications
{
    public class Notification
    {
        public Notification(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }
}
