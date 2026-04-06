using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class AnularReferenciaServiceContext : ReferenciaRecepcionServiceContext, IAnularReferenciaServiceContext
    {
        public HashSet<int> ReferenciasEnUso { get; set; } = new HashSet<int>();

        public HashSet<int> ReferenciaIds { get; set; } = new HashSet<int>();

        public AnularReferenciaServiceContext(IUnitOfWork uow, List<ReferenciaRecepcion> referencias, int userId, int empresa)
            : base(uow, referencias, userId, empresa)
        {
        }

        public async override Task Load()
        {
            await base.Load();

            foreach (var r in Referencias.Values)
            {
                ReferenciaIds.Add(r.Id);
            }

            foreach (var id in _uow.ReferenciaRecepcionRepository.GetReferenciasEnUso(ReferenciaIds))
            {
                ReferenciasEnUso.Add(id);
            }
        }

        public virtual bool ReferenciaEnUso(int referencia)
        {
            return ReferenciasEnUso.Contains(referencia);
        }
    }
}