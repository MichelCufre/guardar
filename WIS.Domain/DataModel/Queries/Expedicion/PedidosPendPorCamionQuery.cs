using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class PedidosPendPorCamionQuery : QueryObject<V_EXP043_PEDIDOS_PEND_CAMION, WISDB>
    {
        protected readonly int? _idCamion;
        public PedidosPendPorCamionQuery()
        {

        }
        public PedidosPendPorCamionQuery(int idCamion)
        {
            this._idCamion = idCamion;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EXP043_PEDIDOS_PEND_CAMION.AsNoTracking();
            if (_idCamion != null)
            {
                this.Query = this.Query.Where(x => x.CD_CAMION == _idCamion);
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
