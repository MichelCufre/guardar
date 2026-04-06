using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class ProductosFinalesNoEsperadosProduccionQuery : QueryObject<V_PRD113_PRODUCTOS_NO_ESPERADOS, WISDB>
	{
		protected readonly string _nroIngresoProduccion;
        protected readonly int _empresa;

		public ProductosFinalesNoEsperadosProduccionQuery(string nroIngresoProduccion, int empresa)
		{
			_nroIngresoProduccion = nroIngresoProduccion;
			_empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD113_PRODUCTOS_NO_ESPERADOS.Where(w => w.NU_PRDC_INGRESO == _nroIngresoProduccion && w.CD_EMPRESA == _empresa);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
