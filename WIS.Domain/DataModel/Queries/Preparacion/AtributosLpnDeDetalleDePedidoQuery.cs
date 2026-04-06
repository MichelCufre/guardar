using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class AtributosLpnDeDetalleDePedidoQuery : QueryObject<V_PRE153_ATRIBUTO_LPN_DETALLE_PEDIDO, WISDB>
	{
		protected readonly string _nuPedido;
		protected readonly string _idLpnExterno;
		protected readonly string _tipoLpn;


		public AtributosLpnDeDetalleDePedidoQuery()
		{

		}

		public AtributosLpnDeDetalleDePedidoQuery(string _nuPedido, string _idLpnExterno, string _tipoLpn)
		{
			this._nuPedido = _nuPedido;
			this._idLpnExterno = _idLpnExterno;
			this._tipoLpn = _tipoLpn;
		}

		public override void BuildQuery(WISDB context)
		{
			this.Query = context.V_PRE153_ATRIBUTO_LPN_DETALLE_PEDIDO.AsNoTracking();

			if (!string.IsNullOrEmpty(_nuPedido))
			{
				this.Query = this.Query
					.Where(x => x.NU_PEDIDO == _nuPedido &&
						   x.ID_LPN_EXTERNO == _idLpnExterno &&
						   x.TP_LPN_TIPO == _tipoLpn);
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
