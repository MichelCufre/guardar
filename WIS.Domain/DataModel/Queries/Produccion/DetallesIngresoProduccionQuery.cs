using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class DetallesIngresoProduccionQuery : QueryObject<V_PRD110_DETALLES_CONSUMIDOS, WISDB>
	{
		protected readonly string _idIngreso;

		public DetallesIngresoProduccionQuery(string idIngreso)
		{
			_idIngreso = idIngreso;
		}

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD110_DETALLES_CONSUMIDOS.Where(w => w.NU_PRDC_INGRESO == _idIngreso);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
