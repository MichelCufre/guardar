using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class TipoVehiculoRequest
    {
        public string CodigoExterno { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int? PesoMaximo { get; set; }
        public int? VolumenMaximo { get; set; }
        public int? CantidadBultosMaxima { get; set; }
        public int? AlturaMaxima { get; set; }
        public bool? AdmiteZorra { get; set; }
        public bool? CargaLateral { get; set; }
        public bool? Frigorificado { get; set; }
        public bool? SoloCabina { get; set; }
    }
}
