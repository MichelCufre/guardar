using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class DetallesContenedoresCompartidos : QueryObject<V_DET_CONT_CON_PROBLEMA, WISDB>
    {
        protected int? _NuContenedor;
        protected int? _NuPreparacion;

        public DetallesContenedoresCompartidos()
        {

        }

        public DetallesContenedoresCompartidos(int nuContenedor, int nuPreparacion)
        {
            this._NuContenedor = nuContenedor;
            this._NuPreparacion = nuPreparacion;
        }

        public override void BuildQuery(WISDB context)
        {
            if (_NuContenedor != null && _NuPreparacion != null)
                this.Query = context.V_DET_CONT_CON_PROBLEMA.Where(w => w.NU_CONTENEDOR == _NuContenedor);
            else
                this.Query = context.V_DET_CONT_CON_PROBLEMA;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
