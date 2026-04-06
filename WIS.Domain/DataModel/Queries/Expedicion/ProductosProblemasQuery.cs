using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ProductosProblemasQuery : QueryObject<V_EXP051_PRODS_PROBLEMAS, WISDB>
    {
        protected readonly string _pedido;
        protected readonly string _respetaOrden;
        protected readonly int? _camion;

        public ProductosProblemasQuery()
        {

        }

        public ProductosProblemasQuery(string pedido, int camion, string respetaOrden)
        {
            this._pedido = pedido;
            this._camion = camion;
            this._respetaOrden = respetaOrden;

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EXP051_PRODS_PROBLEMAS;

            if(!string.IsNullOrEmpty(this._pedido) && this._camion != null)
            {
                this.Query = this.Query.Where(x => x.NU_PEDIDO == this._pedido && x.CD_CAMION == this._camion);
            }
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
