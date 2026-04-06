using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class PrediosQuery : QueryObject<V_PREDIOS, WISDB>
    {
        public PrediosQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PREDIOS.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> GetPredios()
        {
            return this.Query.Select(x => x.NU_PREDIO).ToList();
        }

    }
}
