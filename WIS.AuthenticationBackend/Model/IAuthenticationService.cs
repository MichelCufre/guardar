using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.AuthenticationBackend.Model
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest userData);
    }
}
