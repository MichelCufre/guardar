using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
	public class ControlesDePreaparacionQuery : QueryObject<V_CONTROL_CONT_ENV_WPRE220, WISDB>
	{
		public override void BuildQuery(WISDB context)
		{
			this.Query = context.V_CONTROL_CONT_ENV_WPRE220;
		}

		public virtual int GetCount()
		{
			if (this.Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return this.Query.Count();
		}

		public virtual DateTime? GetMinFechaPrimerControl()
		{
			return this.Query.Where(s => s.DT_PRIMER_CTRL.HasValue).Min(s => s.DT_PRIMER_CTRL);
		}

		public virtual DateTime? GetMaxFechaUltimoControl()
		{
			return this.Query.Where(s => s.DT_ULTIMO_CTRL.HasValue).Max(s => s.DT_ULTIMO_CTRL);
		}
	}
}
