using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class RecursosDisponiblesUsuarioQuery : QueryObject<V_SEG020_RECURSOS, WISDB>
    {
        protected readonly int? _userId;
        public RecursosDisponiblesUsuarioQuery()
        {
        }

        public RecursosDisponiblesUsuarioQuery(int userId)
        {
            this._userId = userId;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SEG020_RECURSOS;
            if (this._userId != null)
            {
                var listIdRecursos = context.V_SEG030_RECURSOS_ASIGNADOS.Where(x => x.USERID == this._userId).Select(x => x.RESOURCEID).ToList();

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
