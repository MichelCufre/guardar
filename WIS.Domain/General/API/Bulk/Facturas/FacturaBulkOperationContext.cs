using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Bulk.Facturas
{
    public class FacturaBulkOperationContext
    {
        public List<object> NewFacturas = new List<object>();

        public List<object> NewDetalles = new List<object>();
    }
}
