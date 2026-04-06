using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class ConsultaEtiquetasQuery : QueryObject<V_REC150_ETIQUETAS, WISDB>
    {
        protected readonly string _idProducto;
        protected readonly string _identificador;
        protected readonly int? _idAgenda;
        protected readonly decimal? _embalaje;
        public ConsultaEtiquetasQuery()
        {
        }
        public ConsultaEtiquetasQuery(int idAgenda, string idProducto, string identificador, decimal embajale)
        {
            this._idAgenda = idAgenda;
            this._identificador = identificador;
            this._idProducto = idProducto;
            this._embalaje = embajale;
        }
        public ConsultaEtiquetasQuery(int idAgenda)
        {
            this._idAgenda = idAgenda;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC150_ETIQUETAS.AsNoTracking();

            if (this._idAgenda != null)
                this.Query = this.Query.Where(x => x.NU_AGENDA == this._idAgenda);

            if ((!string.IsNullOrEmpty(_idProducto)) && (!string.IsNullOrEmpty(_identificador)) && (this._embalaje != null))
                this.Query = this.Query.Where(x => x.NU_IDENTIFICADOR == _identificador && x.CD_FAIXA == _embalaje && x.CD_PRODUTO == _idProducto);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
        public virtual bool AnyEtiquetaAgenda(int idAgenda)
        {
            return this.Query.Any(s => s.NU_AGENDA == idAgenda);
        }
    }
}