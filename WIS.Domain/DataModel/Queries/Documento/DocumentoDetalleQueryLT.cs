using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoDetalleQueryLT : QueryObject<V_LT_DET_DOCUMENTO, WISDB>
    {
        protected readonly string _nuDocumento;
        protected readonly string _tpDocumento;
        protected readonly string _producto;
        protected readonly string _lote;
        protected readonly int _empresa;
        protected readonly decimal _faixa;

        public DocumentoDetalleQueryLT(string producto, string lote, decimal faixa, int empresa, string nuDocumento, string tpDocumento)
        {
            this._nuDocumento = nuDocumento;
            this._tpDocumento = tpDocumento;
            this._producto = producto;
            this._lote = lote;
            this._empresa = empresa;
            this._faixa = faixa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_LT_DET_DOCUMENTO
                .Where(d => d.CD_FAIXA == this._faixa
                    && d.CD_EMPRESA == this._empresa
                    && d.CD_PRODUTO == this._producto
                    && d.NU_IDENTIFICADOR == this._lote
                    && d.NU_DOCUMENTO == this._nuDocumento
                    && d.TP_DOCUMENTO == this._tpDocumento);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
