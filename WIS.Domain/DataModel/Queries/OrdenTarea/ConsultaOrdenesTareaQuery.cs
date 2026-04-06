using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.OrdenTarea
{
    public class ConsultaOrdenesTareaQuery : QueryObject<V_TAREAS_WORT120, WISDB>
    {
        protected string _codigoPlantilla;
        protected short _situacion;

        public ConsultaOrdenesTareaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_TAREAS_WORT120;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
