using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.StockEntities
{
    public class UnidadTransporte
    {
        public int NumeroUnidadTransporte { get; set; }
        public string NumeroExternoUnidad { get; set; }
        public string TipoUnidadTransporte { get; set; }
        public string Ubicacion { get; set; }
        public short? Situacion { get; set; }
        public string CodigoBarras { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string CodigoGrupo { get; set; }
        public string UbicacionDestino { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Ancho { get; set; }
        public decimal? Profundidad { get; set; }
        public string CodigoUnidadBulto { get; set; }
        public string CantidadBultos { get; set; }
        public decimal? CantidadContenedores { get; set; }
    }
}
