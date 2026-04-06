using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaUTQuery : QueryObject<V_STO750_CONSULTA_UT, WISDB>
    {
        protected readonly int? _nroUt;

        public ConsultaUTQuery(int? nroUT = null)
        {
            _nroUt = nroUT;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO750_CONSULTA_UT;

            if (_nroUt.HasValue)
                this.Query = this.Query.Where(u => u.NU_UNIDAD_TRANSPORTE == _nroUt.Value);

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<int> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => r.NU_UNIDAD_TRANSPORTE.ToString())
                .Intersect(keysToSelect)
                .Select(r => int.Parse(r))
                .ToList();
        }

        public virtual List<int> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => r.NU_UNIDAD_TRANSPORTE.ToString())
                .Except(keysToExclude)
                .Select(r => int.Parse(r))
                .ToList();
        }
    }
}
