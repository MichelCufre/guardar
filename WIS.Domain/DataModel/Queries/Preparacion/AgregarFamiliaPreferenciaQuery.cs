using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class AgregarFamiliaPreferenciaQuery : QueryObject<V_PRE811_PREF_FAMILIA, WISDB>
    {
        protected readonly int _preferencia;

        public AgregarFamiliaPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_FAMILIA.AsNoTracking().Where(d => d.NU_PREFERENCIA == this._preferencia && d.NU_PREFERENCIA_FAMILIA == null);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarFamilia> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_FAMILIA_PRODUTO.ToString() }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarFamilia { codFamilia = int.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarFamilia> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_FAMILIA_PRODUTO.ToString() }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarFamilia { codFamilia = int.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }
    }
}
