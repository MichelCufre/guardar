using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarEmpresaPreferenciaQuery : QueryObject<V_PRE811_PREF_EMPRESA, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarEmpresaPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_EMPRESA.AsNoTracking().Where(d => d.NU_PREFERENCIA_EMPRESA != null && d.NU_PREFERENCIA_EMPRESA == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarEmpresa> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_EMPRESA.ToString() }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarEmpresa { cdEmpresa = short.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarEmpresa> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_EMPRESA.ToString() }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarEmpresa { cdEmpresa = short.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }
    }
}