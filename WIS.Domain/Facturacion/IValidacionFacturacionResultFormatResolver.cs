using System.Collections.Generic;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;

namespace WIS.Domain.Facturacion
{
    public interface IValidacionFacturacionResultFormatResolver
    {
        ValidacionCamionResultado Resolve(string message, List<Pedido> pedidos);
    }
}
