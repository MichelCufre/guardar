using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class InsumosSolicitadosEnComunQuery : QueryObject<V_PRD112_INSUMOS_SOLICITADOS_EN_COMUN, WISDB>
	{
		protected readonly string _idEspacio;
		protected readonly List<string> _productos;

		public InsumosSolicitadosEnComunQuery(string idEspacio, List<string> productos)
		{
			_idEspacio = idEspacio;
			_productos = productos;
		}

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD112_INSUMOS_SOLICITADOS_EN_COMUN.Where(w => w.CD_PRDC_LINEA == _idEspacio && _productos.Contains(w.CD_PRODUTO));
		}

		public virtual decimal GetQtForProducto(string cdProducto, string nuIdentificador)
		{
			var filteredQuery = Query.Where(w => w.CD_PRODUTO == cdProducto);

			if (!string.IsNullOrEmpty(nuIdentificador))
			{
				filteredQuery = filteredQuery.Where(w => w.NU_IDENTIFICADOR == nuIdentificador);
			}

			try
			{
				return (decimal)filteredQuery.Sum(s => s.QT_PEDIDO);
			}
			catch (Exception)
			{
				return 0;
			}
		}
	}
}
