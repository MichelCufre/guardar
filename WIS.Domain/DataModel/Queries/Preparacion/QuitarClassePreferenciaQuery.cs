using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarClassePreferenciaQuery : QueryObject<V_PRE811_PREF_CLASSE, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarClassePreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_CLASSE.AsNoTracking().Where(d => d.NU_PREFERENCIA_CLASSE != null && d.NU_PREFERENCIA_CLASSE == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarClase> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_CLASSE }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarClase { codClasse = w, nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarClase> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_CLASSE }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarClase { codClasse = w, nuPreferencia = nuPreferencia }).ToList();
        }
    }
}