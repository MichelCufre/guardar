using System;
using System.Collections.Generic;

namespace WIS.Domain.General.API.Bulks
{
    public class ConfirmacionProduccionBulkOperationContext
    {
        public string NroIngresoProduccion { get; set; }

        public long NuTransaccion { get; set; }

        public long NroInterfazEjecucion { get; set; }

        public short Situacion { get; set; }

        public DateTime? FechaFinProduccion { get; set; }
        public string EspacioProduccion { get; set; }

        public List<object> Insumos = new List<object>();

        public List<object> ProductosFinales = new List<object>();

        public List<object> InsumosMovimiento = new List<object>();

        public List<object> ProductosFinalesMovimiento = new List<object>();

        public ConfirmacionProduccionBulkOperationContext()
        {
            Insumos = new List<object>();
            ProductosFinales = new List<object>();
            InsumosMovimiento = new List<object>();
            ProductosFinalesMovimiento = new List<object>();
        }
    }
}
