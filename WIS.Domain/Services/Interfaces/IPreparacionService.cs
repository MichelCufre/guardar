using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;

namespace WIS.Domain.Services.Interfaces
{
    public interface IPreparacionService
    {
        Task<ValidationsResult> AnularPickingPedidoPendiente(List<AnularPickingPedidoPendiente> ajustes, int userId);
        Task<ValidationsResult> AnularPickingPedidoPendienteAutomatismo(List<AnularPickingPedidoPendienteAutomatismo> ajustes, int userId);
        Task<ValidationsResult> ProcesarPicking(List<DetallePreparacion> pickings, int userId);
    }
}
