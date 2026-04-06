using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class AgregarCondLibPreferenciaQuery : QueryObject<V_PRE811_PREF_COND_LIB, WISDB>
    {
        protected readonly int _preferencia;

        public AgregarCondLibPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_COND_LIB.AsNoTracking().Where(d => d.NU_PREFERENCIA == this._preferencia && d.NU_PREFERENCIA_COND_LIB == null);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarCondLib> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_CONDICION_LIBERACION }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarCondLib { codCondicionLiberacion = w, nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarCondLib> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_CONDICION_LIBERACION }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarCondLib { codCondicionLiberacion = w, nuPreferencia = nuPreferencia }).ToList();
        }
    }
}