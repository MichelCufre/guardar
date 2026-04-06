using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class DiferenciaProducidoQuery : QueryObject<V_PRODUCIDOS_PRODUCCION, WISDB>
	{
		protected readonly string _idIngreso;
        protected readonly int _empresa;

        public DiferenciaProducidoQuery(string idIngreso, int empresa)
		{
			_idIngreso = idIngreso;
			_empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRODUCIDOS_PRODUCCION.Where(f => f.NU_PRDC_INGRESO == _idIngreso && f.FL_DIFERENCIA == "S" && f.CD_EMPRESA == _empresa);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
