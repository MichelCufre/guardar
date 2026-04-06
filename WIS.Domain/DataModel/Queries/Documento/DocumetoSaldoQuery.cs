using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumetoSaldoQuery : QueryObject<V_DET_DOC_DUA_DOC020, WISDB>
    {
        protected readonly string _nuDocumento;
        protected readonly string _tpDocumento;
        protected readonly int? _empresa;
        protected readonly List<string> _tiposDocumento;

        public DocumetoSaldoQuery(string nuDocumento, string tpDocumento, int? empresa, List<string> tiposDocumento)
        {
            this._nuDocumento = nuDocumento;
            this._tpDocumento = tpDocumento;
            this._empresa = empresa;
            this._tiposDocumento = tiposDocumento;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DET_DOC_DUA_DOC020.Where(d => this._tiposDocumento.Contains(d.TP_DOCUMENTO));

            if (!string.IsNullOrEmpty(this._nuDocumento) && !string.IsNullOrEmpty(this._tpDocumento) && this._empresa != null)
                this.Query = this.Query
                    .Where(d => d.NU_DOCUMENTO == this._nuDocumento 
                        && d.TP_DOCUMENTO == this._tpDocumento 
                        && d.CD_EMPRESA == this._empresa);
        }

        public virtual ResultadoSaldosDocumento GetSumaSaldos()
        {
            var lista = this.Query.ToList();

            return new ResultadoSaldosDocumento
            {
                Reservado = lista.Sum(a => a.QT_RESERVADA ?? 0),
                Mercaderia = lista.Sum(a => a.QT_MERCADERIA ?? 0),
                Desafectada = lista.Sum(a => a.QT_DESAFECTADA ?? 0),
                Ingresado = lista.Sum(a => a.QT_INGRESADA ?? 0),
                Disponible = lista.Sum(a => a.QT_DISPONIBLE ?? 0),
                Existencia = lista.Sum(a => a.VL_EXISTENCIA_USD ?? 0)
            };
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
