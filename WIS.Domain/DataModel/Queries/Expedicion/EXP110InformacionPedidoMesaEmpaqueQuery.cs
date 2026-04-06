using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class EXP110InformacionPedidoMesaEmpaqueQuery : QueryObject<V_PEDIDO_EMPAQUE, WISDB>
    {
        protected readonly string _ubicacion;


        public EXP110InformacionPedidoMesaEmpaqueQuery(string ubicacion)
        {
            _ubicacion = ubicacion;

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PEDIDO_EMPAQUE.AsNoTracking()
                .Where(x => x.CD_ENDERECO == _ubicacion || x.TP_EXPEDICION == "BIN");
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
