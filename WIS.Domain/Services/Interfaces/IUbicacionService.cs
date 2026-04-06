using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IUbicacionService
    {
        Task<ValidationsResult> ImportarUbicaciones(List<UbicacionExterna> ubicaciones, IUnitOfWork uow);
    }
}
