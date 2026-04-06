using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class EliminarPedidosLpnPendientesQuery : QueryObject<V_PRE110_DET_PEDIDO_LPN, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE110_DET_PEDIDO_LPN.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR, r.ID_ESPECIFICA_IDENTIFICADOR, r.TP_LPN_TIPO,r.ID_LPN_EXTERNO}))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR, r.ID_ESPECIFICA_IDENTIFICADOR, r.TP_LPN_TIPO, r.ID_LPN_EXTERNO }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList(); //TODO: Ver de cambiar
        }

    }
}
