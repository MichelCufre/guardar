using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IEgresoMapper
    {
        List<Camion> Map(EgresoRequest request);
        EgresoResponse MapToResponse(Camion camion, Dictionary<string, Agente> agentes);
    }
}
