using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class RemanenteProducidoQuery : QueryObject<V_PRD113_REMANENTE_PRODUCIDO, WISDB>
	{
		protected readonly string _ubicacionProduccion;
        protected readonly int _empresa;

        public RemanenteProducidoQuery(string ubicacionProduccion, int empresa)
		{
			_ubicacionProduccion = ubicacionProduccion;
			_empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRD113_REMANENTE_PRODUCIDO.Where(f => f.CD_ENDERECO == _ubicacionProduccion && f.QT_ESTOQUE > 0 && f.CD_EMPRESA == _empresa);
		}

		public virtual int GetCount()
		{
			if (Query == null)
				throw new InvalidOperationException("La query no esta lista para hacer conteo");

			return Query.Count();
		}
	}
}
