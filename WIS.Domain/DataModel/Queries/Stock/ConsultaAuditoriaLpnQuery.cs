using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaAuditoriaLpnQuery : QueryObject<V_STO730_AUDITORIA_LPN, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO730_AUDITORIA_LPN.AsNoTracking();

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
                .Select(r => string.Join("$", new string[] { r.NU_AUDITORIA_AGRUPADOR.ToString() }))
                .Intersect(keysToSelect).Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Where(x => x.ID_ESTADO == EstadoAuditoriaLpn.Pendiente)
                .Select(r => string.Join("$", new string[] { r.NU_AUDITORIA_AGRUPADOR.ToString() }))
                .Except(keysToExclude).Select(w => w.Split('$'))
                .ToList();
        }
    }
}