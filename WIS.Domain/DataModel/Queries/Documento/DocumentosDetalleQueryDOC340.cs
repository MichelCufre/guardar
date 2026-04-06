using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentosDetalleQueryDOC340 : QueryObject<V_DET_DOCUMENTO, WISDB>
    {
        protected readonly string _nuDocumento;
        protected readonly string _tpDocumento;

        public DocumentosDetalleQueryDOC340()
        {

        }

        public DocumentosDetalleQueryDOC340(string nuDocumento, string tpDocumento)
        {
            this._nuDocumento = nuDocumento;
            this._tpDocumento = tpDocumento;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DET_DOCUMENTO;

            if (!string.IsNullOrEmpty(this._nuDocumento) && !string.IsNullOrEmpty(this._tpDocumento))
            {
                this.Query = this.Query.Where(de => de.NU_DOCUMENTO == this._nuDocumento
                    && de.TP_DOCUMENTO == this._tpDocumento);
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
