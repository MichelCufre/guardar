using System;
using System.Globalization;
using WIS.Security;

namespace WIS.TrackingAPI.Services
{
    public class IdentityService : IIdentityService, IIdentityServiceManager
    {
        public int UserId { get; set; }
        private CultureInfo Culture { get; set; }
        public string Application { get; set; }
        public string Predio { get; set; }

        public IdentityService()
        {
        }

        //TODO: Ver de cambiar para evitar mantener estado
        public void SetUser(BasicUserData user, string application, string predio)
        {
            this.UserId = user.UserId;
            this.Culture = new CultureInfo(user.Language);
            this.Application = application;
            this.Predio = predio;
        }

        public IFormatProvider GetFormatProvider()
        {
            return this.Culture;
        }
    }
}
