using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;

namespace WIS.Domain.Facturacion
{
    public class ValidacionFacturacionResultFormatResolver : IValidacionFacturacionResultFormatResolver
    {
        public virtual ValidacionCamionResultado Resolve(string message, List<Pedido> pedidosResolver)
        {
            var pedidos = pedidosResolver.Select(d => $"Pedido: {d.Id} - Cliente:{d.Cliente} - Empresa: {d.Empresa}").ToList();
            return new ValidacionCamionResultado(message, pedidos);
        }
    }
}
