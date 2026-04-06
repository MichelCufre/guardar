using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class REC410DetalleProductoSinClasificarQuery : QueryObject<V_DET_ETIQUETA_SIN_CLASIFICAR, WISDB>
    {
        protected readonly int _nuEtiquetaLote;
        protected readonly string _zona;

        public REC410DetalleProductoSinClasificarQuery(int nuEtiquetaLote)
        {
            this._nuEtiquetaLote = nuEtiquetaLote;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DET_ETIQUETA_SIN_CLASIFICAR
                .Where(s => s.NU_ETIQUETA_LOTE == _nuEtiquetaLote);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
