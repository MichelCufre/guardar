using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class VehiculoEspecificacion
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal? CapacidadVolumen { get; set; }
        public decimal? CapacidadPeso { get; set; }
        public decimal? CapacidadPallet { get; set; }
        public short? PorcentajeOcupacionVolumen { get; set; }
        public short? PorcentajeOcupacionPeso { get; set; }
        public short? PorcentajeOcupacionPallet { get; set; }
        public bool AdmiteCargaLateral { get; set; }
        public bool TieneRefrigeracion { get; set; }
        public bool AdmiteZorra { get; set; }
        public bool TieneSoloCabina { get; set; }
        public bool SincronizacionRealizada { get; set; }
    }
}
