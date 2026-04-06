using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class Componente1Producto : QueryObject<V_REG009_PRODUCTO_COMPONENTE1, WISDB>
    {
        public Componente1Producto()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG009_PRODUCTO_COMPONENTE1.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> GetByNombreOrCodePartial(string value)
        {
            return this.Query.Where(x => x.ND_FACTURACION_COMP1.ToLower().Contains(value.ToLower())).Select(x=>x.ND_FACTURACION_COMP1).ToList();
        }

        public virtual List<string> GetComponentes()
        {
            return this.Query.Select(x => x.ND_FACTURACION_COMP1).ToList();
        }

    }
}
