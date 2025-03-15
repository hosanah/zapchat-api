using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs;

namespace Zapchat.Domain.Interfaces
{
    public interface IAuthService : IDisposable
    {
        Task<dynamic> LoginUserAutenticate(LoginUserRequestDto userRequest);
    }
}
