using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallePedidoAtributoQuery : QueryObject<V_PRE100_DET_PEDIDO_ATRIBUTO, WISDB>
    {
        protected readonly Pedido _pedido;

        public DetallePedidoAtributoQuery(Pedido pedido)
        {
            this._pedido = pedido;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE100_DET_PEDIDO_ATRIBUTO
                .Where(x => x.NU_PEDIDO == this._pedido.Id
                    && x.CD_CLIENTE == this._pedido.Cliente
                    && x.CD_EMPRESA == this._pedido.Empresa);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
