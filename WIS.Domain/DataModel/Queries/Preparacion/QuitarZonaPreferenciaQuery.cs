using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarZonaPreferenciaQuery : QueryObject<V_PRE811_PREF_ZONA, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarZonaPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_ZONA.AsNoTracking().Where(d => d.NU_PREFERENCIA_ZONA != null && d.NU_PREFERENCIA_ZONA == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarZona> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_ZONA }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarZona { codZona = w, nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarZona> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_ZONA }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarZona { codZona = w, nuPreferencia = nuPreferencia }).ToList();
        }
    }
}