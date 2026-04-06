using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class DetallesIngresoControlAuxQuery : QueryObject<V_PRD110_DETALLES_INGRESO, WISDB>
	{
		protected readonly string _idIngreso;
		protected readonly string _tpDetalle;

		public DetallesIngresoControlAuxQuery(string idIngreso, string tpDetalle)
		{
			_idIngreso = idIngreso;
			_tpDetalle = tpDetalle;
		}

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD110_DETALLES_INGRESO.Where(w => w.NU_PRDC_INGRESO == _idIngreso);

			if (!string.IsNullOrEmpty(_tpDetalle)) Query = Query.Where(w => w.TP_REGISTRO == _tpDetalle);

		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
