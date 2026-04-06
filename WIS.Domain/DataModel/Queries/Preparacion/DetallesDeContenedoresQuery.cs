using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallesDeContenedoresQuery : QueryObject<V_PRE061_DET_PREPARACION, WISDB>
    {
        protected int? _preparacion;
        protected int? _contenedor;
        public DetallesDeContenedoresQuery(int preparacion, int contenedor)
        {
            this._preparacion = preparacion;
            this._contenedor = contenedor;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE061_DET_PREPARACION.AsNoTracking();

            if(this._preparacion != null && this._contenedor != null)
            {
                this.Query = this.Query.Where(x => x.NU_PREPARACION == this._preparacion && x.NU_CONTENEDOR == this._contenedor);
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
