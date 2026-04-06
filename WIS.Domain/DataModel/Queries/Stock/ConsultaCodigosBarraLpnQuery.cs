using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
	public class ConsultaCodigosBarraLpnQuery : QueryObject<V_LPN_CODIGOS_BARRAS, WISDB>
	{
		protected readonly long? _nuLpn;

		public ConsultaCodigosBarraLpnQuery()
		{
		}

		public ConsultaCodigosBarraLpnQuery(long? nuLpn)
		{
			_nuLpn = nuLpn;
		}

		public override void BuildQuery(WISDB context)
		{
			this.Query = context.V_LPN_CODIGOS_BARRAS;

			if (this._nuLpn != null)
				this.Query = this.Query.Where(l => l.NU_LPN == _nuLpn);

		}
		public virtual int GetCount()
		{
			if (this.Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return this.Query.Count();
		}
	}
}