using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PedidosLpnDisponiblesQuery : QueryObject<V_PRE100_LPN_DISPONIBLES, WISDB>
    {
        protected readonly Pedido _pedido;

        public PedidosLpnDisponiblesQuery(Pedido pedido)
        {
            this._pedido = pedido;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE100_LPN_DISPONIBLES
                .Where(x => x.NU_PEDIDO == this._pedido.Id
                    && x.CD_EMPRESA == this._pedido.Empresa
                    && x.CD_CLIENTE == this._pedido.Cliente
                    && (!string.IsNullOrEmpty(this._pedido.Predio) ? x.NU_PREDIO == this._pedido.Predio : true) );

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.TP_LPN_TIPO, r.ID_LPN_EXTERNO }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.TP_LPN_TIPO, r.ID_LPN_EXTERNO }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList();
        }
    }
}
