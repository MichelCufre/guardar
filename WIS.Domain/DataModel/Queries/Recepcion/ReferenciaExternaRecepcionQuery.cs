using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class ReferenciaExternaRecepcionQuery : QueryObject<V_REC173_REL_RECEPCION_TP_EMP, WISDB>
    {
        protected readonly int? _empresa;
        public ReferenciaExternaRecepcionQuery()
        {
        }

        public ReferenciaExternaRecepcionQuery(int empresa)
        {
            this._empresa = empresa;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC173_REL_RECEPCION_TP_EMP.AsNoTracking();
            if (this._empresa != null)
                this.Query = this.Query.Where(x => x.CD_EMPRESA == this._empresa);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
