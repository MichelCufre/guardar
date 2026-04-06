using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.StockEntities
{
    public class EtiquetaUnidadTransferencia
    {
        public int NumeroUnidadTransporte { get; set; }
        public string NumeroExternoUnidad { get; set; }
        public string UnidadTransporte { get; set; }
        public string Ubicacion { get; set; }
        public short? Estado { get; set; }
        public string CodigoBarras { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string CodigoGrupo { get; set; }
        public string UbicacionDestino { get; set; }
        public decimal? PS_REAL { get; set; }
        public decimal? VL_ALTURA { get; set; }
        public decimal? VL_LARGURA { get; set; }
        public decimal? VL_PROFUNDIDADE { get; set; }
        public string CD_UNIDAD_BULTO { get; set; }
        public string QT_BULTO { get; set; }
        public decimal? QT_CONTENEDOR { get; set; }
    }
}
