using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class RemanenteInsumosQuery : QueryObject<V_PRD113_STOCK_INSUMOS, WISDB>
	{
		protected readonly string _idIngreso;
        protected readonly int _empresa;

        public RemanenteInsumosQuery(string idIngreso, int empresa)
		{
			_idIngreso = idIngreso;
			_empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD113_STOCK_INSUMOS.Where(f => f.NU_PRDC_INGRESO == _idIngreso && f.QT_REAL > 0 && f.CD_EMPRESA == _empresa);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
