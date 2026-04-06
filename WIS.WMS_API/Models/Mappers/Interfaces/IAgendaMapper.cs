using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Recepcion;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IAgendaMapper
    {
        List<Agenda> Map(AgendasRequest request);
        AgendaResponse MapToResponse(Agenda agenda, string cdAgente, string tpAgente);
    }
}
