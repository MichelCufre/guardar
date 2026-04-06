using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Automatismo.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Automatizacion
{
    public class CaracteristicasAutomatismoQuery : QueryObject<V_AUT100_CARACTERISTICAS, WISDB>
    {
        protected readonly int numeroAutomatismo;
        protected readonly bool hasSingleValue;

        public CaracteristicasAutomatismoQuery(int numeroAutomatismo, bool hasSingleValue = false)
        {
            this.numeroAutomatismo = numeroAutomatismo;
            this.hasSingleValue = hasSingleValue;
        }

        public override void BuildQuery(WISDB context)
        {
            var query = context.V_AUT100_CARACTERISTICAS.AsNoTracking().Where(i => i.NU_AUTOMATISMO == numeroAutomatismo);

            if (hasSingleValue) this.Query = query.Where(i => i.VL_OPCIONES != null && i.VL_OPCIONES != AutomatismoDb.LISTA_CARACTERISTICA_AUTOMATISMO);

            else this.Query = query.Where(i => i.VL_OPCIONES == AutomatismoDb.LISTA_CARACTERISTICA_AUTOMATISMO || string.IsNullOrEmpty(i.VL_OPCIONES));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
