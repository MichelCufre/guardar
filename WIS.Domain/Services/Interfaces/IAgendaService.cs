using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAgendaService
    {
        Task<ValidationsResult> AgregarAgendas(int empresa, List<Agenda> agendas, int userId);
        Task<Agenda> GetAgenda(int nuAgenda);
    }
}
