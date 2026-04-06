using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class OrdenTareaEquipoQuery : QueryObject<V_ORT080_ORDEN_TAREA_EQUIPO, WISDB>
    {
        protected long _nuOrdenTarea;

        public OrdenTareaEquipoQuery()
        {
        }

        public OrdenTareaEquipoQuery(long nuOrdenTarea)
        {
            this._nuOrdenTarea = nuOrdenTarea;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ORT080_ORDEN_TAREA_EQUIPO.AsNoTracking().Select(d => d);

            if (_nuOrdenTarea != -1)
                this.Query = this.Query.Where(o => o.NU_ORDEN_TAREA == _nuOrdenTarea);
            else
                this.Query = context.V_ORT080_ORDEN_TAREA_EQUIPO;

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
