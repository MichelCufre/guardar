using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarControlAccesoPreferenciaQuery : QueryObject<V_PRE811_PREF_CONT_ACCESO, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarControlAccesoPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_CONT_ACCESO.AsNoTracking().Where(d => d.NU_PREFERENCIA_CONT_ACCESO != null && d.NU_PREFERENCIA_CONT_ACCESO == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarContolAcceso> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_CONTROL_ACCESO.ToString() }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarContolAcceso { cdControlAcceso = w, nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarContolAcceso> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_CONTROL_ACCESO.ToString() }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarContolAcceso { cdControlAcceso = w, nuPreferencia = nuPreferencia }).ToList();
        }
    }
}