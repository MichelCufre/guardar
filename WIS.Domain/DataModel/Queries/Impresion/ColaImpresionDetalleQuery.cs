using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Impresion
{
    public class ColaImpresionDetalleQuery : QueryObject<V_LIMP010DET_IMPRESION, WISDB>
    {
        protected readonly int? _impresion;
        public ColaImpresionDetalleQuery()
        {
        }
        public ColaImpresionDetalleQuery(int impresion)
        {
            this._impresion = impresion;
        }


        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_LIMP010DET_IMPRESION.AsNoTracking();

            if (this._impresion != null)
                this.Query = this.Query.Where(x => x.NU_IMPRESION == this._impresion);

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
