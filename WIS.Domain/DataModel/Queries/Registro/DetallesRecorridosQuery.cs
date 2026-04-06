using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class DetallesRecorridosQuery : QueryObject<V_REG700_RECORRIDO_DET, WISDB>
    {
        protected readonly int? _nuRecorrido;

        public DetallesRecorridosQuery()
        {
        }

        public DetallesRecorridosQuery(int nuRecorrido)
        {
            this._nuRecorrido = nuRecorrido;
        }

        public override void BuildQuery(WISDB context)
        {
            Query = context.V_REG700_RECORRIDO_DET.Select(d => d);

            if (_nuRecorrido != null)
                Query = Query.Where(i => i.NU_RECORRIDO == _nuRecorrido);
        }

        public virtual int GetCount()
        {
            if (Query == null) throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return Query.Count();
        }
    }
}
