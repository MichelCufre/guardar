using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoAjustesStockDOC361Query : QueryObject<V_DOC361_AJUSTES_STOCK, WISDB>
    {
        protected bool _ajustePositivo;
        protected int _empresa;

        public DocumentoAjustesStockDOC361Query(int empresa, bool ajustePositivo)
        {
            this._empresa = empresa;
            this._ajustePositivo = ajustePositivo;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._ajustePositivo)
            {
                this.Query = context.V_DOC361_AJUSTES_STOCK
                    .Where(a => a.CD_EMPRESA == this._empresa && a.QT_MOVIMIENTO > 0);
            }
            else
            {
                this.Query = context.V_DOC361_AJUSTES_STOCK
                    .Where(a => a.CD_EMPRESA == this._empresa && a.QT_MOVIMIENTO < 0);
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
