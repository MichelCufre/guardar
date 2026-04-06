using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IPreparacionMapper
    {
        List<AnularPickingPedidoPendiente> Map(AnularPickingPedidoPendienteRequest request);
        List<AnularPickingPedidoPendienteAutomatismo> MapAutomatismo(AnularPickingPedidoPendienteRequest request);
        List<DetallePreparacion> Map(PickingRequest request, int userId);
    }
}
