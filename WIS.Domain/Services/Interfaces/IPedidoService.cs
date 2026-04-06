using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;

namespace WIS.Domain.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<ValidationsResult> AgregarPedidos(List<Pedido> pedidos, int userId);
        Task<Pedido> GetPedido(string nuPedido, int empresa, string tipoAgente, string codigoAgente);
        Task<ValidationsResult> ModificarPedidos(List<Pedido> pedidos, int userId);

        Task<TrackingValidationResult> ActualizarPedidosPuntoEntrega(PuntoEntregaAgentes puntoEntrega, string loginName);
    }
}
