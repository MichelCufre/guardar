using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Security;

namespace WIS.Application.Security
{
    public class IdentityService : IIdentityService, IIdentityServiceManager
    {
        public int UserId { get; set; }
        protected CultureInfo Culture { get; set; }
        public string Application { get; set; }
        public string Predio { get; set; }

        public IdentityService()
        {
        }

        //TODO: Ver de cambiar para evitar mantener estado
        public virtual void SetUser(BasicUserData user, string application, string predio)
        {
            this.UserId = user.UserId;
            this.Culture = new CultureInfo(user.Language);
            this.Application = application;
            this.Predio = predio;
        }

        public virtual IFormatProvider GetFormatProvider()
        {
            return this.Culture;
        }
    }
}
