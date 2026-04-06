using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class EtiquetasUTDeRecepcionQuery : QueryObject<V_REC170_ETIQUETA_UT, WISDB>
    {
        protected readonly int? _numAgenda;
        public EtiquetasUTDeRecepcionQuery()
        {
        }

        public EtiquetasUTDeRecepcionQuery(int numAgenda)
        {
            this._numAgenda = numAgenda;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC170_ETIQUETA_UT.AsNoTracking();
            if (this._numAgenda != null)
                this.Query = this.Query.Where(x => x.NU_AGENDA == this._numAgenda);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}