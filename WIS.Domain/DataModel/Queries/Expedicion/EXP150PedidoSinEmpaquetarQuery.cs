using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class EXP150PedidoSinEmpaquetarQuery : QueryObject<V_PEDIDO_PREPARADO_COMPLETO, WISDB>
    {
        public EXP150PedidoSinEmpaquetarQuery()
        {

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PEDIDO_PREPARADO_COMPLETO.AsNoTracking();

            this.Query = this.Query.Where(x => x.FL_EMPAQUETA_CONTENEDOR == "S" && x.QT_CONTENEDOR > 0);
        }
    }
}
