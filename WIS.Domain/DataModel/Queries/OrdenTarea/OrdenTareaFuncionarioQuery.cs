using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class OrdenTareaFuncionarioQuery : QueryObject<V_ORT_ORDEN_TAREA_FUNC_WORT060, WISDB>
    {
        protected long _nuOrdenTarea;

        public OrdenTareaFuncionarioQuery()
        {
        }

        public OrdenTareaFuncionarioQuery(long nuOrdenTarea)
        {
            this._nuOrdenTarea = nuOrdenTarea;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ORT_ORDEN_TAREA_FUNC_WORT060.AsNoTracking().Select(d => d);

            if (_nuOrdenTarea != -1)
                this.Query = this.Query.Where(o => o.NU_ORDEN_TAREA == _nuOrdenTarea);
            else
                this.Query = context.V_ORT_ORDEN_TAREA_FUNC_WORT060;

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
