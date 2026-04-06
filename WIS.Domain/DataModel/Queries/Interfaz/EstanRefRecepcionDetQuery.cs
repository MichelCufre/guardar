using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Interfaz
{
    public class EstanRefRecepcionDetQuery : QueryObject<V_INT103_ESTAN_REF_RECEPC_DET, WISDB>
    {
        protected readonly int _nroInterfaz;

        public EstanRefRecepcionDetQuery(int nroInterfaz)
        {
            this._nroInterfaz = nroInterfaz;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INT103_ESTAN_REF_RECEPC_DET.Where(i => i.NU_INTERFAZ_EJECUCION == _nroInterfaz);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
