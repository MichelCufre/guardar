using WIS.Automation;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;

namespace WIS.AutomationManager.Interfaces
{
    public interface IAutomatismoInterpreterClientService
    {
        AutomatismoResponse SendProductos(IAutomatismo automatismo, ProductosAutomatismoRequest request);
        AutomatismoResponse SendCodigosBarras(IAutomatismo automatismo, CodigosBarrasAutomatismoRequest request);
        AutomatismoResponse SendEntrada(IAutomatismo automatismo, EntradaStockAutomatismoRequest request);
        AutomatismoResponse SendSalida(IAutomatismo automatismo, SalidaStockAutomatismoRequest request);
    }
}
