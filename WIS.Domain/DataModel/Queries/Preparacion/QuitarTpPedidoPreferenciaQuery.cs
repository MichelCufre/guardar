using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class QuitarTpPedidoPreferenciaQuery : QueryObject<V_PRE811_PREF_TP_PEDIDO, WISDB>
    {
        protected readonly int _preferencia;

        public QuitarTpPedidoPreferenciaQuery(int preferencia)
        {
            this._preferencia = preferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE811_PREF_TP_PEDIDO.AsNoTracking().Where(d => d.NU_PREFERENCIA_TP_PEDIDO != null && d.NU_PREFERENCIA_TP_PEDIDO == this._preferencia);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<PreferenciaAsociarTpPedido> GetSelectedKeys(List<string> keysToSelect, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.TP_PEDIDO }))
                                       .Intersect(keysToSelect).Select(w => new PreferenciaAsociarTpPedido { tpPedido = w, nuPreferencia = nuPreferencia }).ToList();
        }

        public virtual List<PreferenciaAsociarTpPedido> GetSelectedKeysAndExclude(List<string> keysToExclude, int nuPreferencia)
        {
            return this.GetResult().Select(r => string.Join("$", new string[] { r.TP_PEDIDO }))
                                        .Except(keysToExclude).Select(w => new PreferenciaAsociarTpPedido { tpPedido = w, nuPreferencia = nuPreferencia }).ToList();
        }
    }
}