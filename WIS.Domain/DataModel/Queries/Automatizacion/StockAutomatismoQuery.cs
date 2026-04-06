using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Automatizacion
{
    public class StockAutomatismoQuery : QueryObject<V_STOCK_AUTOMATISMO_AUT100, WISDB>
    {
        protected readonly string _tipoUbicacion;

        public StockAutomatismoQuery(string tipoUbicacion)
        {
            this._tipoUbicacion = tipoUbicacion;
        }

        public override void BuildQuery(WISDB context)
        {
            if (!string.IsNullOrEmpty(this._tipoUbicacion))
                this.Query = context.V_STOCK_AUTOMATISMO_AUT100.AsNoTracking().Where(w => w.ND_TIPO_ENDERECO == this._tipoUbicacion);
            else
                this.Query = context.V_STOCK_AUTOMATISMO_AUT100.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
