using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoAgrupableDOC330Query : QueryObject<V_DOCUMENTO_AGRUPABLE_DOC330, WISDB>
    {
        protected string _nuAgrupador;
        protected string _tpAgrupador;
        protected List<string> _tpDocumentoHabilitados;
        protected int? _empresa;

        public DocumentoAgrupableDOC330Query(string nuAgrupador, string tpAgrupador, List<string> tpDocumentoHabilitados, int? empresa)
        {
            this._nuAgrupador = nuAgrupador;
            this._tpAgrupador = tpAgrupador;
            this._tpDocumentoHabilitados = tpDocumentoHabilitados;
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._empresa != null)
            {
                this.Query = context.V_DOCUMENTO_AGRUPABLE_DOC330
                    .Where(d => this._tpDocumentoHabilitados.Contains(d.TP_DOCUMENTO) && d.CD_EMPRESA == this._empresa)
                    .OrderByDescending(d => string.IsNullOrEmpty(d.NU_AGRUPADOR) && string.IsNullOrEmpty(d.TP_AGRUPADOR))
                    .ThenByDescending(d => d.NU_AGRUPADOR == _nuAgrupador && d.TP_AGRUPADOR == this._tpAgrupador)
                    .ThenByDescending(d => d.DT_ADDROW);
            }
            else
            {
                this.Query = context.V_DOCUMENTO_AGRUPABLE_DOC330
                    .Where(d => this._tpDocumentoHabilitados.Contains(d.TP_DOCUMENTO))
                    .OrderByDescending(d => string.IsNullOrEmpty(d.NU_AGRUPADOR) && string.IsNullOrEmpty(d.TP_AGRUPADOR))
                    .ThenByDescending(d => d.NU_AGRUPADOR == _nuAgrupador && d.TP_AGRUPADOR == this._tpAgrupador)
                    .ThenByDescending(d => d.DT_ADDROW);
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
