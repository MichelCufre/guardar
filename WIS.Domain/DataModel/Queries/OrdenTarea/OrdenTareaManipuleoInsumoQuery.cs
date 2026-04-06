using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class OrdenTareaManipuleoInsumoQuery : QueryObject<V_ORT_ORDEN_TAREA_DATO_WORT070, WISDB>
    {
        protected long _nuOrdenTarea;

        public OrdenTareaManipuleoInsumoQuery()
        {
        }

        public OrdenTareaManipuleoInsumoQuery(long nuOrdenTarea)
        {
            this._nuOrdenTarea = nuOrdenTarea;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ORT_ORDEN_TAREA_DATO_WORT070.AsNoTracking().Select(d => d);

            if (_nuOrdenTarea != -1)
                this.Query = this.Query.Where(o => o.NU_ORDEN_TAREA == _nuOrdenTarea);
            else
                this.Query = context.V_ORT_ORDEN_TAREA_DATO_WORT070;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
