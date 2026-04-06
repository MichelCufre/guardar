using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class DetalleDeReferenciaDeRecepcion : QueryObject<V_REC011_REC_REFERENCIA_DET, WISDB>
    {
        protected int? _idReferencia;
        public DetalleDeReferenciaDeRecepcion()
        {
        }

        public DetalleDeReferenciaDeRecepcion(int? idReferencia)
        {
            this._idReferencia = idReferencia;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC011_REC_REFERENCIA_DET.AsNoTracking();
            if(_idReferencia != null)
            {
                this.Query = this.Query.Where(x => x.NU_RECEPCION_REFERENCIA == _idReferencia);
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