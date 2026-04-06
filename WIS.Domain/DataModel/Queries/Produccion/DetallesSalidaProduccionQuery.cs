using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class DetallesSalidaProduccionQuery : QueryObject<V_PRD110_DETALLES_PRODUCIDOS, WISDB>
	{
		protected readonly string _idIngreso;

		public DetallesSalidaProduccionQuery(string idIngreso)
		{
			_idIngreso = idIngreso;
		}

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD110_DETALLES_PRODUCIDOS.Where(w => w.NU_PRDC_INGRESO == _idIngreso);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
