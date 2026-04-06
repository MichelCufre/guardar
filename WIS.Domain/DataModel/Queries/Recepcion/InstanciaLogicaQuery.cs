using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class InstanciaLogicaQuery : QueryObject<V_REC275_LOGICA_INSTANCIA, WISDB>
    {
        protected int _numeroEstrategia;
        public InstanciaLogicaQuery()
        {
        }

        public InstanciaLogicaQuery(int numeroEstrategia)
        {
            this._numeroEstrategia = numeroEstrategia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC275_LOGICA_INSTANCIA.AsNoTracking();

            if (this._numeroEstrategia != 0)
            {
                this.Query = this.Query.Where(d => d.NU_ALM_ESTRATEGIA == this._numeroEstrategia);
            }
            else
            {
                this.Query = context.V_REC275_LOGICA_INSTANCIA;
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