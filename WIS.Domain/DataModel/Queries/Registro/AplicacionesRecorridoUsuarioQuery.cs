using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class AplicacionesRecorridoUsuarioQuery : QueryObject<V_APLICACION_RECORRIDO_USUARIO, WISDB>
    {
        public AplicacionesRecorridoUsuarioQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            Query = context.V_APLICACION_RECORRIDO_USUARIO;
        }

        public virtual int GetCount()
        {
            if (Query == null) throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return Query.Count();
        }
    }
}
