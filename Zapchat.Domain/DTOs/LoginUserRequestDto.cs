using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs
{
    public class LoginUserRequestDto
    {
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
