using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class CrossDockingDetailQuery : QueryObject<V_REC210_CROSS_DOCKING, WISDB>
    {
        protected readonly int? _empresa;
        protected readonly int? _agenda;

        public CrossDockingDetailQuery(int empresa, int agenda)
        {
            this._empresa = empresa;
            this._agenda = agenda;
        }
        public CrossDockingDetailQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC210_CROSS_DOCKING.AsNoTracking();

            if (_agenda != null && _empresa != null)
            {
                this.Query = this.Query.Where(d => d.CD_EMPRESA == this._empresa && d.NU_AGENDA == this._agenda);
            }
            this.Query = this.Query.Where(d => ((d.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto && d.QT_PRODUTO > 0) || (d.NU_IDENTIFICADOR != ManejoIdentificadorDb.IdentificadorAuto)));

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
