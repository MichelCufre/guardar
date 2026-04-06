using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.Produccion.Queries
{
	public class ProductosFinalesProduccionQuery : QueryObject<V_PRODUCTOS_FINALES_PRODUCCION, WISDB>
	{
		protected readonly string _nroIngresoProduccion;
		public ProductosFinalesProduccionQuery(string nroIngresoProduccion)
		{
			_nroIngresoProduccion = nroIngresoProduccion;
		}

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRODUCTOS_FINALES_PRODUCCION.Where(w => w.NU_PRDC_INGRESO == _nroIngresoProduccion);
		}
	}
}
