using System;

namespace WIS.Domain.Produccion
{
    public class FormulaConfiguracion
    {
        public int Pasada { get; set; }
        public int Orden { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public FormulaAccion Accion { get; set; }
    }
}
