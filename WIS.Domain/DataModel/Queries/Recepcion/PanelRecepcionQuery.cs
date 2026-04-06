using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class PanelRecepcionQuery : QueryObject<V_REC170_RECEPCION, WISDB>
    {
        protected readonly int? _referencia;
        protected readonly List<int> _numAgendas;
        public PanelRecepcionQuery()
        {

        }
        public PanelRecepcionQuery(int idReferencia)
        {
            this._referencia = idReferencia;
        }
        public PanelRecepcionQuery(List<int> numAgendas)
        {
            this._numAgendas = numAgendas;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC170_RECEPCION.AsNoTracking();

            if (this._referencia != null)
            {
                this.Query = this.Query.Where(x => x.NU_AGENDA == this._referencia);
            }
            if (this._numAgendas != null && this._numAgendas.Count > 0)
            {
                this.Query = this.Query.Where(x => this._numAgendas.Contains(x.NU_AGENDA));
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