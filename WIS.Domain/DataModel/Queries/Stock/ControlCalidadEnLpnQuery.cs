using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ControlCalidadEnLpnQuery : QueryObject<V_STO153_CTRL_CALIDAD_DET_LPN, WISDB>
    {
        public ControlCalidadEnLpnQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO153_CTRL_CALIDAD_DET_LPN.Where(x =>x.QT_RESERVA_SAIDA == 0);
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
                .Select(r => string.Join("$", new string[] { r.NU_LPN.ToString(), r.ID_LPN_DET.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_LPN.ToString(), r.ID_LPN_DET.ToString() }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList();
        }
    }
}
