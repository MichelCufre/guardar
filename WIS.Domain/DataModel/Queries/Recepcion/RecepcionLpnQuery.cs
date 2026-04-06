using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class RecepcionLpnQuery : QueryObject<V_REC170_RECEPCION_LPNS, WISDB>
    {
        protected readonly int? _nuAgenda;

        public RecepcionLpnQuery(int? nuAgenda = null)
        {
            _nuAgenda = nuAgenda;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC170_RECEPCION_LPNS;

            if (_nuAgenda.HasValue)
                this.Query = this.Query.Where(d => d.NU_AGENDA == _nuAgenda.Value);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
