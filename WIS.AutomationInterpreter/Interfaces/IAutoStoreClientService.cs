using WIS.Automation;
using WIS.Domain.Integracion.Dtos;

namespace WIS.AutomationInterpreter.Interfaces
{
    public interface IAutoStoreClientService
    {
        AutomatismoResponse SendProductos(AutomatismoInterpreterRequest request);
        AutomatismoResponse SendCodigosBarras(AutomatismoInterpreterRequest request);
        AutomatismoResponse SendEntrada(AutomatismoInterpreterRequest request);
        AutomatismoResponse SendSalida(AutomatismoInterpreterRequest request);
    }
}
