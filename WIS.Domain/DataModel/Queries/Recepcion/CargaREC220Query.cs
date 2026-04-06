using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class CargaREC220Query : QueryObject<V_CARGA_WREC220, WISDB>
    {
        protected List<long> listaCargas;

        public CargaREC220Query(List<long> listaCargas)
        {
            this.listaCargas = listaCargas;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CARGA_WREC220.AsNoTracking();
            if (listaCargas != null && listaCargas.Count() > 0)
            {
                this.Query = this.Query.Where(w => listaCargas.Contains(w.NU_CARGA));

            }
            else
            {
                this.Query = this.Query.Where(c => c.CD_ROTA == -1);
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
