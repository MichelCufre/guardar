using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class AgenteRequest
    {
        public string Codigo { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
