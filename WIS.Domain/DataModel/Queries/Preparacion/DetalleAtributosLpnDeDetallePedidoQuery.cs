using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class DetalleAtributosLpnDeDetallePedidoQuery : QueryObject<V_PRE154_DETALLE_ATRIBUTOS_LPN_DETALLE_PEDIDO, WISDB>
	{
		protected readonly long _nuDetalle;

		public DetalleAtributosLpnDeDetallePedidoQuery()
		{

		}

		public DetalleAtributosLpnDeDetallePedidoQuery(long _nuDetalle)
		{
			this._nuDetalle = _nuDetalle;
		}

		public override void BuildQuery(WISDB context)
		{
			this.Query = context.V_PRE154_DETALLE_ATRIBUTOS_LPN_DETALLE_PEDIDO.AsNoTracking();

			if (this._nuDetalle != 0)
			{
				this.Query = this.Query
					.Where(x => x.NU_DET_PED_SAI_ATRIB == _nuDetalle);
			}
		}

		public virtual int GetCount()
		{
			if (this.Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return this.Query.Count();
		}

	}
}
