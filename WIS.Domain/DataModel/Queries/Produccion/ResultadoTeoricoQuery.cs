using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class ResultadoTeoricoQuery : QueryObject<V_PRD112_RESULTADO_TEORICO, WISDB>
	{
		string _idIngreso;

		public ResultadoTeoricoQuery(string idIngreso)
		{
			_idIngreso = idIngreso;
		}

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD112_RESULTADO_TEORICO.Where(w => w.NU_PRDC_INGRESO == _idIngreso);
		}
	}
}
