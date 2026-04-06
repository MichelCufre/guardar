using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class AnalisisRechazoLpnQuery : QueryObject<V_PRE170_ANALISIS_RECHAZO_LPN, WISDB>
	{
		protected readonly int? _nuPreparacion;
		public AnalisisRechazoLpnQuery()
		{

		}
		public AnalisisRechazoLpnQuery(int nuPrepa)
		{
			this._nuPreparacion = nuPrepa;
		}
		public override void BuildQuery(WISDB context)
		{
			this.Query = context.V_PRE170_ANALISIS_RECHAZO_LPN;
			if (_nuPreparacion != null)
			{
				this.Query = this.Query.Where(x => x.NU_PREPARACION == _nuPreparacion);
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
