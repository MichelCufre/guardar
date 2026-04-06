using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PanelPreferenciasQuery : QueryObject<V_PRE811_PREFERENCIAS, WISDB>
    {
        protected int? _nuPreferencia;

        public PanelPreferenciasQuery()
        {
        }
        public PanelPreferenciasQuery(int nuPreferencia)
        {
            this._nuPreferencia = nuPreferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREFERENCIAS.AsNoTracking();

            if (this._nuPreferencia != null)
            {
                this.Query = this.Query.Where(d => d.NU_PREFERENCIA == this._nuPreferencia);
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
