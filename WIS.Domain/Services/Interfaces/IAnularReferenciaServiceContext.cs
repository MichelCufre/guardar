using System.Collections.Generic;
using System.Threading.Tasks;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAnularReferenciaServiceContext : IReferenciaRecepcionServiceContext
    {
        HashSet<int> ReferenciasEnUso { get; set; }
        HashSet<int> ReferenciaIds { get; set; }

        Task Load();

        bool ReferenciaEnUso(int referencia);
    }
}