using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallePedidoSalidaQuery : QueryObject<V_PRE150_DET_PEDIDO_SALIDA, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE150_DET_PEDIDO_SALIDA;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> GetPedidosPendientesPreparar(int numPreparacionManual)
        {
            return this.Query
                .Where(x => x.NU_PREPARACION_MANUAL == numPreparacionManual
                    && x.QT_PENDIENTE > 0)
                .Select(w => $"{w.NU_PEDIDO} - {w.CD_CLIENTE}")
                .ToList();
        }
    }
}
