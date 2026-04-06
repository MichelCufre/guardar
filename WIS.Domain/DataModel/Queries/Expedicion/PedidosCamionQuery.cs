using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
	public class PedidosCamionQuery : QueryObject<V_PEDIDOS_CAMION_EXP050, WISDB>
    {
        protected int _cdCamion;

        public PedidosCamionQuery(int cdCamion)
        {
            _cdCamion = cdCamion;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PEDIDOS_CAMION_EXP050.AsNoTracking().Where(s => s.CD_CAMION == _cdCamion);
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_CAMION.ToString(), r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_CAMION.ToString(), r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString() }))
                .Except(keysToExclude).Select(w => w.Split('$'))
                .ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
