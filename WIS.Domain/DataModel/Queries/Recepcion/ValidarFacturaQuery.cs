using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class ValidarFacturaQuery : QueryObject<V_REC170_VALIDAR_FACTURA, WISDB>
    {
        protected readonly int? _idAgenda;
        public ValidarFacturaQuery(int idAgenda)
        {
            this._idAgenda = idAgenda;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC170_VALIDAR_FACTURA.AsNoTracking();
            if (_idAgenda != null)
            {
                this.Query = this.Query.Where(x => x.NU_AGENDA == this._idAgenda);
            }

        }
        public virtual new int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
