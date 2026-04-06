using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DOC500Query : QueryObject<V_CAMBIO_DOC_DOC500, WISDB>
    {
        protected readonly string _nuDocumento;
        protected readonly string _tpDocumento;
        protected readonly int? _empresa;


        public DOC500Query(string nuDocumento, string tpDocumento, int? empresa)
        {
            this._nuDocumento = nuDocumento;
            this._tpDocumento = tpDocumento;
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CAMBIO_DOC_DOC500;

            if (!string.IsNullOrEmpty(this._nuDocumento) && !string.IsNullOrEmpty(this._tpDocumento) && this._empresa != null)
                this.Query = this.Query.Where(d => d.NU_DOCUMENTO_EGRESO_PRDC == this._nuDocumento 
                    && d.TP_DOCUMENTO_EGRESO_PRDC == this._tpDocumento 
                    && d.CD_EMPRESA == this._empresa);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
