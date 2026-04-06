using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.TrafficOfficer
{
    public class TrafficOfficerResponse
    {
        public TrafficOfficerResponseStatus Estado { get; set; }
        public string Message { get; set; }
        public string CodErrorPadre { get; set; }
        public string CodErrorDetallado { get; set; }
        public ThreadOperation DataResponse { get; set; }

        public bool IsErrorRegistroBloqueado()
        {
            return this.CodErrorDetallado == TrafficOfficerErrorCode.RegistroBloqueado;
        }
    }
}
