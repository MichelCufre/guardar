using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaLogsAtributosDetalleLpnQuery : QueryObject<V_STO722_HISTORIAL_ATRIBUTO_DETALLE, WISDB>
    {
        protected int? _idDetalleLpn;

        public ConsultaLogsAtributosDetalleLpnQuery(int? idDetalleLpn = null)
        {
            this._idDetalleLpn = idDetalleLpn;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO722_HISTORIAL_ATRIBUTO_DETALLE
                .AsNoTracking()
                .Where(d => _idDetalleLpn.HasValue ? d.ID_LPN_DET == this._idDetalleLpn : true);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}