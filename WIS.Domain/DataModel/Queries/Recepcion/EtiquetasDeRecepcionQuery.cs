using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class EtiquetasDeRecepcionQuery : QueryObject<V_REC190_ETIQUETA_LOTE, WISDB>
    {
        protected readonly int? _numAgenda;
        public EtiquetasDeRecepcionQuery()
        {
        }

        public EtiquetasDeRecepcionQuery(int numAgenda)
        {
            this._numAgenda = numAgenda;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC190_ETIQUETA_LOTE.AsNoTracking();
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