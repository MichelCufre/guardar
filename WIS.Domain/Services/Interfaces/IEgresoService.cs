using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Expedicion;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IEgresoService
    {
        Task<ValidationsResult> AgregarEgresos(int empresa, List<Camion> egresos, int userId);
        Task<Camion> GetCamion(int cdCamion);
        Task<Camion> GetCamionByIdExterno(string idExterno, int empExterna);
    }
}
