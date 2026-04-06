using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Authentication;

namespace WIS.AuthenticationService.Model
{
    public class SecretProvider : ISecretProvider
    {
        private readonly string _secretKey;

        public SecretProvider(IOptions<AppSettings> settings)
        {
            this._secretKey = settings.Value.Secret;
        }

        public string GetSecret()
        {
            return this._secretKey;
        }
    }
}
