using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaLogsDetallesAtributosQuery : QueryObject<V_STO710_LT_LPN_ATRIBUTO_DETALLE, WISDB>
    {
        protected long _numeroLPN;
        protected string _idDetalle;

        public ConsultaLogsDetallesAtributosQuery(long numeroLPN, string idDetalle = null)
        {
            this._numeroLPN = numeroLPN;
            this._idDetalle = idDetalle;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO710_LT_LPN_ATRIBUTO_DETALLE
                .AsNoTracking()
                .Where(d => d.NU_LPN == this._numeroLPN);

            if (int.TryParse(this._idDetalle, out int idLpnDet))
                this.Query = this.Query.Where(d => d.ID_LPN_DET == idLpnDet);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}