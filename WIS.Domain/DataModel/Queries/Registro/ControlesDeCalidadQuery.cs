using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class ControlesDeCalidadQuery : QueryObject<V_REG601_CONTROLES_CALIDAD, WISDB>
    {
        public ControlesDeCalidadQuery()
        {
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG601_CONTROLES_CALIDAD.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}