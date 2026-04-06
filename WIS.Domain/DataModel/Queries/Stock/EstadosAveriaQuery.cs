using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
	public class EstadosAveriaQuery : QueryObject<V_CLASIFICACION_STOCK_WSTO640, WISDB>
    {
        public EstadosAveriaQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CLASIFICACION_STOCK_WSTO640.AsNoTracking();
        }

        public virtual List<string> GetKeysRowsSelected(bool allSelected, List<string> keys)
        {
            if (allSelected)
            {
                return this.GetResult()
                .Select(s => s.NU_LOG_CLASIF_STOCK.ToString())
                .Except(keys)
                .ToList();
            }
            else
            {
                return this.GetResult()
                .Select(s => s.NU_LOG_CLASIF_STOCK.ToString())
                .Intersect(keys)
                .ToList();
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
