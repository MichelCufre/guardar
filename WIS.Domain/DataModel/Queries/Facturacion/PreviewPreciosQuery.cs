using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class PreviewPreciosQuery : QueryObject<V_FACTURACION_RESULTA_WFAC012, WISDB>
    {
        protected int? _nuEjecucion;
        protected short _situacion;
        public PreviewPreciosQuery(int? nuEjecucion, short situacion)
        {
            this._nuEjecucion = nuEjecucion;
            this._situacion = situacion;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_FACTURACION_RESULTA_WFAC012.Where(w => w.CD_SITUACAO != this._situacion);

            if (_nuEjecucion != null)
                this.Query = this.Query.Where(w => w.NU_EJECUCION == this._nuEjecucion);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
