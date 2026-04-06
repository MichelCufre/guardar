using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WIS.AuthenticationService.Model
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, CancellationToken cancelToken);
    }
}
