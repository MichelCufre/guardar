using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Interfaz
{
    public class EstanPedidoAnuladoQuery : QueryObject<V_INT104_ESTAN_PEDIDO_ANULADO, WISDB>
    {
        protected readonly int _nroInterfaz;

        public EstanPedidoAnuladoQuery(int nroInterfaz)
        {
            this._nroInterfaz = nroInterfaz;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INT104_ESTAN_PEDIDO_ANULADO.Where(i => i.NU_INTERFAZ_EJECUCION == _nroInterfaz);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
