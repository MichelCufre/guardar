using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class DetallesProducirQuery : QueryObject<V_PRD113_PRODUCIR, WISDB>
	{
		protected readonly string _idIngreso;
        protected readonly int _empresa;

        public DetallesProducirQuery(string idIngreso, int empresa)
		{
			_idIngreso = idIngreso;
			_empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD113_PRODUCIR.Where(w => w.NU_PRDC_INGRESO == _idIngreso && w.CD_EMPRESA == _empresa);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
