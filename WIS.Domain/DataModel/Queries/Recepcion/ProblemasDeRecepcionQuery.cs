using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class ProblemasDeRecepcionQuery : QueryObject<V_REC141_AGENDA_PROBLEMA, WISDB>
    {
        protected readonly int? _idAgenda;
        protected readonly decimal? _idEmbalaje;
        protected readonly string _idProducto;
        protected readonly string _identificador;

        public ProblemasDeRecepcionQuery()
        {
        }
        public ProblemasDeRecepcionQuery(int idAgenda)
        {
            this._idAgenda = idAgenda;
        }
        public ProblemasDeRecepcionQuery(int idAgenda, decimal idEmbalaje, string idProducto, string identificador)
        {
            this._idAgenda = idAgenda;
            this._idEmbalaje = idEmbalaje;
            this._idProducto = idProducto;
            this._identificador = identificador;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC141_AGENDA_PROBLEMA.AsNoTracking();
            if( (_idAgenda != null) && (_idEmbalaje != null) && !(string.IsNullOrEmpty(_idProducto)) && !(string.IsNullOrEmpty(_identificador))){
             
                this.Query = this.Query.Where(x => x.CD_PRODUTO == _idProducto && x.NU_AGENDA == _idAgenda && x.CD_FAIXA == _idEmbalaje && x.NU_IDENTIFICADOR == _identificador);
            }else if(_idAgenda != null)
            {
                this.Query = this.Query.Where(x => x.NU_AGENDA == _idAgenda);
            }

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<int> GetProblemasNoAceptados()
        {
            return this.Query.Where(x => x.FL_ACEPTADO == "N").Select(d => d.NU_RECEPCION_AGENDA_PROBLEMA).ToList();
        }
    }
}