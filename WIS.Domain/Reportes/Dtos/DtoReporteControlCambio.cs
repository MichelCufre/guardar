using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoReporteControlCambio
    {
        public int Preparacion { get; set; }
        public int NroContenedor { get; set; }
        public int Camion { get; set; }
        public int? CantidadBulto { get; set; }
        public string DescripcionTipoContenedor { get; set; }
        public string Matricula { get; set; }
        public int Transportadora { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string TipoCliente { get; set; }
        public string Agente { get; set; }
        public string DescripcionCliente { get; set; }
        public string DescripcionTransportadora { get; set; }
        public string IdExternoContenedor { get; set; }
        public long? NroLpn { get; set; }
    }
}
