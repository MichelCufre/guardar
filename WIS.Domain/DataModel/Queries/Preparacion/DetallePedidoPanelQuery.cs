using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallePedidoPanelQuery : QueryObject<V_PRE101_DET_PEDIDO_SAIDA, WISDB>
    {
        protected readonly int _empresa;
        protected readonly string _pedido;
        protected readonly string _cliente;

        public DetallePedidoPanelQuery(int empresa, string cliente, string pedido)
        {
            this._empresa = empresa;
            this._cliente = cliente;
            this._pedido = pedido;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE101_DET_PEDIDO_SAIDA;

            if (!string.IsNullOrEmpty(this._cliente) && this._empresa != null && !string.IsNullOrEmpty(this._pedido))
                this.Query = this.Query.Where(x => x.NU_PEDIDO == this._pedido && x.CD_EMPRESA == this._empresa && x.CD_CLIENTE == this._cliente);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
