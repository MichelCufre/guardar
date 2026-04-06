using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class AceptarRechazarCalculosQuery : QueryObject<V_FACTURACION_RESULT_WFAC002, WISDB>
    {
        protected int _nuEjecucion;
        public AceptarRechazarCalculosQuery(int nuEjecucion = -1)
        {
            this._nuEjecucion = nuEjecucion;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._nuEjecucion != -1)
                this.Query = context.V_FACTURACION_RESULT_WFAC002.Where(w => w.NU_EJECUCION == this._nuEjecucion);
            else
                this.Query = context.V_FACTURACION_RESULT_WFAC002;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_EJECUCION.ToString(), r.CD_EMPRESA.ToString(), r.CD_FACTURACION, r.NU_COMPONENTE }))
                .Intersect(keysToSelect).Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_EJECUCION.ToString(), r.CD_EMPRESA.ToString(), r.CD_FACTURACION, r.NU_COMPONENTE }))
                .Except(keysToExclude).Select(w => w.Split('$'))
                .ToList();
        }
    }
}
