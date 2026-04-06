using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class LogsDeControlesDePreparacionQuery : QueryObject<V_LOG_CONTROL_PICKEO_WPRE221, WISDB>
	{
		protected readonly int? _prep;
		protected readonly int? _emp;

		public LogsDeControlesDePreparacionQuery()
		{
		}

		public LogsDeControlesDePreparacionQuery(int? prep, int? emp)
		{
			_prep = prep;
			_emp = emp;
		}

		public override void BuildQuery(WISDB context)
		{
			this.Query = context.V_LOG_CONTROL_PICKEO_WPRE221.AsNoTracking();

			if (_prep != null && _emp != null)
			{
				this.Query = this.Query.Where(x => x.NU_PREPARACION == _prep && x.CD_EMPRESA == _emp);
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
