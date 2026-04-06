using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoReporteContCamionDet
    {
        public int Camion { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public string TipoContenedor { get; set; }
        public string DescripcionTipoContenedor { get; set; }
        public string Agente { get; set; }
        public string DescripcionCliente { get; set; }
        public string TipoCliente { get; set; }
        public decimal? CantidadTipoContenedor { get; set; }
    }
}
