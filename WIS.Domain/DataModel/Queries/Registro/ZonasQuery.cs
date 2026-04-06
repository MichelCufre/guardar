using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class ZonasQuery : QueryObject<V_REG070_ZONA_UBICACION, WISDB>
    {
        public ZonasQuery()
        {
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG070_ZONA_UBICACION.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> GetZonas(List<string> zonasNoUsadas)
        {
            return this.Query
                .ToList()
                .Join(zonasNoUsadas,
                    z => new { z.CD_ZONA_UBICACION },
                    znu => new { CD_ZONA_UBICACION = znu },
                    (z, znu) => z)
                .Select(d => d.CD_ZONA_UBICACION)
                .ToList();
        }
    }
}
