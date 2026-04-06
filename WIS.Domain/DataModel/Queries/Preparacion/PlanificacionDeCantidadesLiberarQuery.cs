using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class PlanificacionDeCantidadesLiberarQuery : QueryObject<V_PLANIFICACION_LIB_WPRE670, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PLANIFICACION_LIB_WPRE670;
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_EMPRESA.ToString(), r.CD_CLIENTE, r.CD_PRODUTO, r.NU_IDENTIFICADOR, r.CD_FAIXA.ToString(formatProvider), r.ID_ESPECIFICA_IDENTIFICADOR }))
                .Except(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_EMPRESA.ToString(), r.CD_CLIENTE, r.CD_PRODUTO, r.NU_IDENTIFICADOR, r.CD_FAIXA.ToString(formatProvider), r.ID_ESPECIFICA_IDENTIFICADOR }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
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
