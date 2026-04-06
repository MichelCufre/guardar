using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class RecursosAsignadosUsuarioQuery : QueryObject<V_SEG030_RECURSOS_ASIGNADOS, WISDB>
    {
        protected readonly int? _userId;
        public RecursosAsignadosUsuarioQuery()
        {
        }

        public RecursosAsignadosUsuarioQuery(int userId)
        {
            this._userId = userId;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SEG030_RECURSOS_ASIGNADOS;
            if (this._userId != null)
            {
                var listIdRecursos = context.V_SEG030_RECURSOS_ASIGNADOS.Where(x => x.USERID == this._userId).Select(x => x.RESOURCEID).ToList();

                this.Query = this.Query.Where(x => listIdRecursos.Contains(x.RESOURCEID) && x.USERID == this._userId);
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
