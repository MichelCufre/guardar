using System.Collections.Generic;
using WIS.Domain.Interfaces;

namespace WIS.Domain.Recepcion
{
    public class CrearDetalleFacturaStrategy : ICrearDetallesFacturaStrategy
    {
        protected List<FacturaDetalle> _detalles;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detallesFactura"> Detalles digitados de factura</param>
        public CrearDetalleFacturaStrategy()
        {

        }

        public virtual List<FacturaDetalle> CrearDetallesFactura()
        {
            return new List<FacturaDetalle>();
        }
    }
}
