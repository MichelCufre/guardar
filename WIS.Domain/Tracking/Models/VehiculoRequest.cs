using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class VehiculoRequest
    {
        public string CodigoExterno { get; set; }
        public string Descripcion { get; set; }
        public string CodigoExternoTipoVehiculo { get; set; }
        public string Matricula { get; set; }
        public string Estado { get; set; }
    }
}
