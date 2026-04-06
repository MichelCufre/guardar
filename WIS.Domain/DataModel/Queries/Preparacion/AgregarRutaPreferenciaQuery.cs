using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;
namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class AgregarRutaPreferenciaQuery : QueryObject<V_PRE811_PREF_ROTA, WISDB>
    {
        protected readonly int _preferencia;

        public AgregarRutaPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_ROTA.AsNoTracking().Where(d => d.NU_PREFERENCIA == this._preferencia && d.NU_PREFERENCIA_ROTA == null);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarRuta> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_ROTA.ToString() }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarRuta { codRuta = int.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarRuta> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_ROTA.ToString() }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarRuta { codRuta = int.Parse(w), nuPreferencia = nuPreferencia }).ToList();
        }
    }
}