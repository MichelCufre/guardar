using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
	public class ConsultaDeCargasAsignadasQuery : QueryObject<V_PED_PREP_CAMION_DOC_WEXP090, WISDB>
	{
		public ConsultaDeCargasAsignadasQuery()
		{
		}

		public override void BuildQuery(WISDB context)
		{

			this.Query = context.V_PED_PREP_CAMION_DOC_WEXP090;
		}
		public virtual int GetCount()
		{
			if (this.Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return this.Query.Count();
		}
	}
}
