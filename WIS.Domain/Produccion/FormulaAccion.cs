using System;
using System.Collections.Generic;
using WIS.Domain.Produccion.Enums;

namespace WIS.Domain.Produccion
{
    public class FormulaAccion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public FormulaAccionTipo Tipo { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public List<string> Parametros { get; set; }

        public FormulaAccion()
        {
            this.Parametros = new List<string>();
        }
    }
}
