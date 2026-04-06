using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class EXP052Query : QueryObject<V_EGRESO_CAMION_WEXP, WISDB>
    {
        protected int _idCamion;

        public EXP052Query(int idCamion)
        { 
            this._idCamion = idCamion;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EGRESO_CAMION_WEXP
                .AsNoTracking()
                .Where(e => e.CD_CAMION == this._idCamion);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
