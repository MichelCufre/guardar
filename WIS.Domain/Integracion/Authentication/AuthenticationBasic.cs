using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http.Headers;
using System.Text;
using WIS.Domain.DataModel.Mappers.Integracion;
using WIS.Domain.Integracion.Enums;

namespace WIS.Domain.Integracion.Authentication
{
    public class AuthenticationBasic : IAuthenticationMethod
    {
        private string _username { get; set; }
        private string _password { get; set; }

        public AuthenticationBasic(string username, string password) : base()
        {
            _username = username;
            _password = password;
        }

        public AuthenticationHeaderValue GetAuthorizationHeaderValue(IHttpContextAccessor httpContextAccessor)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this._username, this._password));
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }
    }
}
