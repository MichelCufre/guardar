using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class ProductosSalidaContenedorQuery : QueryObject<V_PRDC_PROD_SALIDA_PREP, WISDB>
    {

        protected readonly int _preparacion;
        protected readonly int _contenedor;

        public ProductosSalidaContenedorQuery(int preparacion, int contenedor)
        {
            this._preparacion = preparacion;
            this._contenedor = contenedor;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_PROD_SALIDA_PREP
                .AsNoTracking()
                .Where(w => w.NU_PREPARACION == _preparacion 
                    && w.NU_CONTENEDOR == _contenedor);

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
