using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarClientePreferenciaQuery : QueryObject<V_PRE811_PREF_CLIENTE, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarClientePreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_CLIENTE.AsNoTracking().Where(d => d.NU_PREFERENCIA_CLIENTE != null && d.NU_PREFERENCIA_CLIENTE == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarCliente> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_CLIENTE.ToString(), r.CD_EMPRESA.ToString() }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarCliente { cdCliente = w.Split('$')[0], cdEmpresa = int.Parse(w.Split('$')[1]), nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarCliente> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.CD_CLIENTE.ToString(), r.CD_EMPRESA.ToString() }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarCliente { cdCliente = w.Split('$')[0], cdEmpresa = int.Parse(w.Split('$')[1]), nuPreferencia = nuPreferencia }).ToList();
        }
    }
}