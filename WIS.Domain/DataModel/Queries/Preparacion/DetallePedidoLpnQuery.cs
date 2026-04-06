using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallePedidoLpnQuery : QueryObject<V_PRE100_DET_PEDIDO_LPN, WISDB>
    {
        protected readonly Pedido _pedido;

        public DetallePedidoLpnQuery(Pedido pedido)
        {
            this._pedido = pedido;
        }


        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE100_DET_PEDIDO_LPN
                .Where(x => x.NU_PEDIDO == this._pedido.Id
                    && x.CD_EMPRESA == this._pedido.Empresa
                    && x.CD_CLIENTE == this._pedido.Cliente);

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
