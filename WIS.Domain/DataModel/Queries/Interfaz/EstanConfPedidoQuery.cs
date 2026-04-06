using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel.Queries.Interfaz
{
    public class EstanConfPedidoQuery : QueryObject<V_INT107_ESTAN_CONF_PED_PEDIDO, WISDB>
    {
        protected readonly int _nroInterfaz;

        public EstanConfPedidoQuery(int nroInterfaz)
        {
            this._nroInterfaz = nroInterfaz;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INT107_ESTAN_CONF_PED_PEDIDO.Where(i => i.NU_INTERFAZ_EJECUCION == _nroInterfaz);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
