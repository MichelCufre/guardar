using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class IngresoResultadosManualesQuery : QueryObject<V_FACTUR_RESULT_WFAC007, WISDB>
    {
        protected int _nuEjecucion;
        protected string _nombrePantalla;
        public IngresoResultadosManualesQuery(int nuEjecucion, string nombrePantalla)
        {
            this._nuEjecucion = nuEjecucion;
            this._nombrePantalla = nombrePantalla;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._nombrePantalla == "FAC007")
            {
                this.Query = context.V_FACTUR_RESULT_WFAC007.Where(w => w.NU_EJECUCION == this._nuEjecucion && w.TP_PROCESO == "M");
            }
            else if (this._nombrePantalla == "FAC009")
            { 
                this.Query = context.V_FACTUR_RESULT_WFAC007.Where(w => w.NU_EJECUCION == this._nuEjecucion);
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
