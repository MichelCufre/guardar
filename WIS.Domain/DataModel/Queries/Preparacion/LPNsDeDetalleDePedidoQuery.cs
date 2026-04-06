using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class LPNsDeDetalleDePedidoQuery : QueryObject<V_PRE152_DETALLE_PEDIDO_LPN, WISDB>
	{
		protected readonly string _nuPedido;
		protected readonly string _cdCliente;
		protected readonly int _cdEmpresa;
		protected readonly string _cdProducto;
		protected readonly decimal _cdFaixa;
		protected readonly string _nuIdentificador;
		protected readonly string _idEspecificaIden;

		public LPNsDeDetalleDePedidoQuery()
		{

		}

		public LPNsDeDetalleDePedidoQuery(string _nuPedido, string _cdCliente, int _cdEmpresa, string _cdProducto, decimal _cdFaixa, string _nuIdentificador, string _idEspecificaIden)
		{
			this._nuPedido = _nuPedido;
			this._cdCliente = _cdCliente;
			this._cdEmpresa = _cdEmpresa;
			this._cdProducto = _cdProducto;
			this._cdFaixa = _cdFaixa;
			this._nuIdentificador = _nuIdentificador;
			this._idEspecificaIden = _idEspecificaIden;
		}

		public override void BuildQuery(WISDB context)
		{
			this.Query = context.V_PRE152_DETALLE_PEDIDO_LPN.AsNoTracking();

			if (!string.IsNullOrEmpty(_nuPedido))
			{
				this.Query = this.Query
					.Where(x => x.NU_PEDIDO == _nuPedido && 
						   x.CD_CLIENTE == _cdCliente && 
						   x.CD_EMPRESA == _cdEmpresa &&
						   x.CD_PRODUTO == _cdProducto &&
						   x.CD_FAIXA == _cdFaixa &&
						   x.NU_IDENTIFICADOR == _nuIdentificador &&
						   x.ID_ESPECIFICA_IDENTIFICADOR == _idEspecificaIden);
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
