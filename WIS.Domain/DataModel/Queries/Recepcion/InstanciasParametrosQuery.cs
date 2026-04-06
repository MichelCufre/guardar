using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class InstanciasParametrosQuery : QueryObject<V_REC275_PARAMETROS_INSTANCIAS, WISDB>
    {
        protected int _numeroInstancia;

        public InstanciasParametrosQuery(int numeroInstancia = 0)
        {
            this._numeroInstancia = numeroInstancia;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC275_PARAMETROS_INSTANCIAS.AsNoTracking();

            if (this._numeroInstancia != 0)
            {
                this.Query = this.Query.Where(d => d.NU_ALM_LOGICA_INSTANCIA == this._numeroInstancia);
            }
            else
            {
                this.Query = context.V_REC275_PARAMETROS_INSTANCIAS;
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