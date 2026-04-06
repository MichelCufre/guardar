using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DetallesAgrupadorQuery : QueryObject<V_DOCUMENTO_MOV_MERC_DOC350, WISDB>
    {
        protected readonly string _nuAgrupador;
        protected readonly string _tpAgrupador;

        public DetallesAgrupadorQuery()
        {

        }

        public DetallesAgrupadorQuery(string nuAgrupador, string tpAgrupador)
        {
            this._nuAgrupador = nuAgrupador;
            this._tpAgrupador = tpAgrupador;
        }


        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOCUMENTO_MOV_MERC_DOC350;

            if (!string.IsNullOrEmpty(this._nuAgrupador) && !string.IsNullOrEmpty(this._tpAgrupador))
            {
                this.Query = this.Query.Where(de => de.NU_AGRUPADOR == this._nuAgrupador
                    && de.TP_AGRUPADOR == this._tpAgrupador);
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