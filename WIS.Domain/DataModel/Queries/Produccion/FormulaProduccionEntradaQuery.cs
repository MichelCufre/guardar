using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class FormulaProduccionEntradaQuery : QueryObject<V_PRDC_DET_ENTRADA_KIT101, WISDB>
    {
        protected readonly string _formula;

        public FormulaProduccionEntradaQuery(string formula)
        {
            this._formula = formula;
        }

        public override void BuildQuery(WISDB context)
        {
            if (string.IsNullOrEmpty(this._formula))
                this.Query = context.V_PRDC_DET_ENTRADA_KIT101
                    .Where(d => d.CD_PRDC_DEFINICION == null);
            else
                this.Query = context.V_PRDC_DET_ENTRADA_KIT101
                    .Where(d => d.CD_PRDC_DEFINICION == this._formula);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
