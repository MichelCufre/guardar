using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarEquipoPreferenciaQuery : QueryObject<V_PRE811_PREF_EQUIPO, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarEquipoPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_EQUIPO.AsNoTracking().Where(d => d.NU_PREFERENCIA_EQUIPO != null && d.NU_PREFERENCIA_EQUIPO == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarEquipo> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_EQUIPO.ToString() }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarEquipo { codEquipo = short.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarEquipo> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_EQUIPO.ToString() }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarEquipo { codEquipo = short.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }
    }
}