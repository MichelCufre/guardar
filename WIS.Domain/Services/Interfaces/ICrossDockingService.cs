using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Picking.Dtos;

namespace WIS.Domain.Services.Interfaces
{
    public interface ICrossDockingService
    {
        Task<ValidationsResult> CrossDockingUnaFase(List<CrossDockingUnaFase> ajustes, int userId);

    }
}
