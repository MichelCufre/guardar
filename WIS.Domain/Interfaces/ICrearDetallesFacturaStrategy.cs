using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Recepcion;

namespace WIS.Domain.Interfaces
{
    public interface ICrearDetallesFacturaStrategy
    {
        List<FacturaDetalle> CrearDetallesFactura();
    }
}
