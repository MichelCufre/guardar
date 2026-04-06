using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class ConsultaGeneralContenedoresQuery : QueryObject<V_PRE060_CONTENEDOR, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE060_CONTENEDOR.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<Contenedor> GetInfoContenedoresSearch(string value)
        {
            List<Contenedor> informacion = new List<Contenedor>();

            var query = this.Query.Where(x => x.NU_CONTENEDOR.ToString().Contains(value.ToLower()) || x.ID_EXTERNO_CONTENEDOR.ToLower().Contains(value.ToLower()));

            foreach (var conte in query)
            {
                informacion.Add(new Contenedor
                {
                    TipoContenedor = conte.TP_CONTENEDOR,
                    IdExterno = conte.ID_EXTERNO_CONTENEDOR,
                    Numero = conte.NU_CONTENEDOR,
                    DescripcionContenedor = conte.DS_CONTENEDOR
                });
            }

            return informacion;
        }
    }
}
