using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class EtiquetaLogQuery : QueryObject<V_REC180_LOG_ETIQUETAS, WISDB>
    {
        protected readonly string _idProducto;
        protected readonly string _identificador;
        protected readonly int? _etiquetaLote;
        protected readonly decimal? _embalaje;
        public EtiquetaLogQuery()
        {
        }
        public EtiquetaLogQuery(int etiquetaLote, decimal embalaje, string idProducto, string identificador)
        {
            this._idProducto = idProducto;
            this._identificador = identificador;
            this._embalaje = embalaje;
            this._etiquetaLote = etiquetaLote;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC180_LOG_ETIQUETAS.AsNoTracking();
            if ((_etiquetaLote != null) && (_embalaje != null) && !(string.IsNullOrEmpty(_idProducto)) && !(string.IsNullOrEmpty(_identificador)))
            {

                this.Query = this.Query.Where(x => x.CD_PRODUTO == _idProducto && x.NU_ETIQUETA_LOTE == _etiquetaLote && x.CD_FAIXA == _embalaje && x.NU_IDENTIFICADOR == _identificador);
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