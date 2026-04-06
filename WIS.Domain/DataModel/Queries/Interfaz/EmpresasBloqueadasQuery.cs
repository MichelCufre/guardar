using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Interfaz
{
	public class EmpresasBloqueadasQuery : QueryObject<V_INT050_EMPRESAS_BLOQUEADAS, WISDB>
    {
        public EmpresasBloqueadasQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INT050_EMPRESAS_BLOQUEADAS.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => r.CD_EMPRESA.ToString())
                .Intersect(keysToSelect)
                .Select(w => w)
                .ToList();
        }

        public virtual List<string> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => r.CD_EMPRESA.ToString())
                .Except(keysToExclude)
                .Select(w => w)
                .ToList();
        }
    }
}
