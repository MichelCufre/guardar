using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.Produccion.Queries
{
	public class EspacioProduccionQuery : QueryObject<V_PRDC_LINEA_KIT190, WISDB>
	{
		public EspacioProduccionQuery()
		{
		}

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRDC_LINEA_KIT190
				.Select(d => d);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
