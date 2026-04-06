using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class NotificationError
    {
        public string Resource { get; set; }
        public List<string> Arguments { get; set; }

    }
}
