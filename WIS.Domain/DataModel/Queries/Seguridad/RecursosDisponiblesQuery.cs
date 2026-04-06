using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class RecursosDisponiblesQuery : QueryObject<V_SEG020_RECURSOS, WISDB>
    {
        protected readonly int? _profileId;
        public RecursosDisponiblesQuery()
        {
        }

        public RecursosDisponiblesQuery(int profileId)
        {
            this._profileId = profileId;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SEG020_RECURSOS;
            if (this._profileId != null)
            {
                var listIdRecursos = context.V_SEG020_RECURSOS_ASIGNADOS.Where(x => x.PROFILEID == this._profileId).Select(x => x.RESOURCEID).ToList();

                this.Query = this.Query.Where(x => !listIdRecursos.Contains(x.RESOURCEID));
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
