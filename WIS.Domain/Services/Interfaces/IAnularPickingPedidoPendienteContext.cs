using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Picking;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAnularPickingPedidoPendienteContext : IServiceContext
    {
        Task Load();

        bool AnyAnulacionPreparacionPendiente(int preparacion);
        bool AnyPreparacionPedido(int preparacion, string pedido, int empresa, string cliente);
        bool ExistePredio(string predio);
        Agente GetAgente(int empresa, string codigo, string tipo);
        List<DetallePreparacion> GetDetallesPreparacionPedido(int preparacion, string pedido, int empresa, string cliente, string estadoPicking);
        Pedido GetPedido(string pedido, int empresa, string cliente);
        bool IsEmpresaDocumental(int empresa);
    }
}