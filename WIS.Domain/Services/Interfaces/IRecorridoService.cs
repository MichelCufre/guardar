using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Recorridos;

namespace WIS.Domain.Services.Interfaces
{
    public interface IRecorridoService
    {
        Task<ValidationsResult> ImportarDetalles(IUnitOfWork uow, List<DetalleRecorrido> detallesRecorrido, int userId, int empresa);
    }
}
