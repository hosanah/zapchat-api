using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapchat.Domain.Interfaces
{
    public interface IUtilsService
    {
        Task<TResponse> ExecuteApiCall<TRequest, TResponse>(HttpMethod httpMethod, Uri fullUrl, TRequest request);
    }
}
