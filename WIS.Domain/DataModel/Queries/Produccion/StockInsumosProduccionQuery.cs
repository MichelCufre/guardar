using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Produccion.Models;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class StockInsumosProduccionQuery : QueryObject<V_PRD113_STOCK_INSUMOS, WISDB>
	{
		protected readonly string _nroIngresoProduccion;
		protected readonly bool _insumosConsumidos;
		protected readonly int _empresa;

		public StockInsumosProduccionQuery(string nroIngresoProduccion, int empresa, bool insumosConsumidos = false)
		{
			_nroIngresoProduccion = nroIngresoProduccion;
			_empresa = empresa;
			_insumosConsumidos = insumosConsumidos;
		}

		public override void BuildQuery(WISDB context)
		{
			if (_insumosConsumidos)
				Query = context.V_PRD113_STOCK_INSUMOS.Where(w => w.NU_PRDC_INGRESO == _nroIngresoProduccion && w.CD_EMPRESA == _empresa);
			else
				Query = context.V_PRD113_STOCK_INSUMOS.Where(w => w.NU_PRDC_INGRESO == _nroIngresoProduccion && w.QT_REAL > 0 && w.CD_EMPRESA == _empresa);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}

		public virtual List<IngresoProduccionDetalleReal> GetStockInsumo()
		{
			return this.Query.Select(d => new IngresoProduccionDetalleReal
			{
				NuPrdcIngresoReal = d.NU_PRDC_INGRESO_REAL,
				NuPrdcIngreso = d.NU_PRDC_INGRESO,
				Producto = d.CD_PRODUTO,
				Empresa = d.CD_EMPRESA,
				Identificador = d.NU_IDENTIFICADOR,
				Consumible = d.FL_CONSUMIBLE,
				QtReal = d.QT_REAL
			}).ToList();
		}
	}
}
