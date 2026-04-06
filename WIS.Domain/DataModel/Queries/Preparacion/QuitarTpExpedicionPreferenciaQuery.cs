using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarTpExpedicionPreferenciaQuery : QueryObject<V_PRE811_PREF_TP_EXP, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarTpExpedicionPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_TP_EXP.AsNoTracking().Where(d => d.NU_PREFERENCIA_TP_EXP != null && d.NU_PREFERENCIA_TP_EXP == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarTpExpedicion> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.TP_EXPEDICION }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarTpExpedicion { tpExpedicion = w, nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarTpExpedicion> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.TP_EXPEDICION }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarTpExpedicion { tpExpedicion = w, nuPreferencia = nuPreferencia }).ToList();
        }
    }
}