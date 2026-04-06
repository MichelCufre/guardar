using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class EliminarPedidosAtributosPendientesQuery : QueryObject<V_PRE110_DET_PEDIDO_ATRIB, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE110_DET_PEDIDO_ATRIB.AsNoTracking();
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
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR, r.ID_ESPECIFICA_IDENTIFICADOR , r.NU_DET_PED_SAI_ATRIB.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR, r.ID_ESPECIFICA_IDENTIFICADOR,r.NU_DET_PED_SAI_ATRIB.ToString() }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList(); //TODO: Ver de cambiar
        }

    }
}
