using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class TipoVehiculoQuery : QueryObject<V_REG250_TIPO_VEHICULO, WISDB>
    {
        public TipoVehiculoQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG250_TIPO_VEHICULO;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
