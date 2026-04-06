using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaDetalleAtributoLpnQuery : QueryObject<V_STO730_DET_ATRIBUTO_LPN, WISDB>
    {
        protected long numeroAuditoria;

        public ConsultaDetalleAtributoLpnQuery(long numeroAuditoria)
        {
            this.numeroAuditoria = numeroAuditoria;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO730_DET_ATRIBUTO_LPN.AsNoTracking().Where(x =>x.NU_AUDITORIA == numeroAuditoria);

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}