using System;
using WIS.Domain.Produccion.Enums;

namespace WIS.Domain.Produccion
{
    public class LineaWhiteBox : ILinea
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public string UbicacionEntrada { get; set; }
        public string UbicacionSalida { get; set; }
        public TipoProduccionLinea Tipo { get; set; }
        public string Predio { get; set; }
        public string NumeroIngreso { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public LineaWhiteBox()
        {
            this.Tipo = TipoProduccionLinea.WhiteBox;
        }
    }
}
