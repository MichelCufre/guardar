using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class PreparacionesNoAnularSelectedQuery : QueryObject<V_PREP_NO_ANULAR_SEL_WPRE450, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PREP_NO_ANULAR_SEL_WPRE450;
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.NU_PEDIDO, r.NU_PREPARACION.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.NU_PEDIDO, r.NU_PREPARACION.ToString() }))
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
