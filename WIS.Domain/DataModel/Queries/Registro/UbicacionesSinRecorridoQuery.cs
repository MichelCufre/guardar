using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class UbicacionesSinRecorridoQuery : QueryObject<V_REG700_UBIC_SIN_RECORRIDOS, WISDB>
    {
        protected readonly int numeroRecorrido;

        public UbicacionesSinRecorridoQuery(int numeroRecorrido)
        {
            this.numeroRecorrido = numeroRecorrido;
        }

        public override void BuildQuery(WISDB context)
        {
            Query = context.V_REG700_UBIC_SIN_RECORRIDOS.Where(i=> i.NU_RECORRIDO == numeroRecorrido);
        }

        public virtual int GetCount()
        {
            if (Query == null) throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return Query.Count();
        }
    }
}
