using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class ReferenciaRecepcionDetalleQuery : QueryObject<V_REC011_REC_REFERENCIA_DET, WISDB>
    {
        protected readonly int? _idReferencia;

        public ReferenciaRecepcionDetalleQuery()
        {
        }
        public ReferenciaRecepcionDetalleQuery(int? idReferencia)
        {
            this._idReferencia = idReferencia;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._idReferencia != null)
            {

                this.Query = context.V_REC011_REC_REFERENCIA_DET.Where(d => d.NU_RECEPCION_REFERENCIA == (int)this._idReferencia).Select(d => d);
            }
            else
            {
                this.Query = context.V_REC011_REC_REFERENCIA_DET.Select(d => d);
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}