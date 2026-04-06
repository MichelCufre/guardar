using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IPickingProductoService
    {
        Task<ValidationsResult> AgregarUbicacionesDePicking(List<UbicacionPickingProducto> ubicacionesPicking, int userId);
        void NotificarAutomatismo(IUnitOfWork uow, List<UbicacionPickingProducto> ubicaciones);
    }
}
