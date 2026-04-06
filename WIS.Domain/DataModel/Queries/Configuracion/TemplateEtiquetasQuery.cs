using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Configuracion
{
    public class TemplateEtiquetasQuery : QueryObject<V_COF030_TEMPLATE_ETIQUETA, WISDB>
    {
        public TemplateEtiquetasQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_COF030_TEMPLATE_ETIQUETA;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
