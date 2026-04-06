using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Configuracion
{
    public class ParametrosConfiguracionQuery : QueryObject<V_LPARAMETROS_CONFIG_LCON020, WISDB>
    {
        protected string _cdParametro;
        protected string _doEntidad;


        public ParametrosConfiguracionQuery(string cdParametro, string doEntidad)
        {
            this._cdParametro = cdParametro;
            this._doEntidad = doEntidad;
        }

        public override void BuildQuery(WISDB context)
        {
            if (!string.IsNullOrEmpty(this._cdParametro) && !string.IsNullOrEmpty(this._doEntidad))
                this.Query = context.V_LPARAMETROS_CONFIG_LCON020.Where(w => w.CD_PARAMETRO == this._cdParametro && w.DO_ENTIDAD_PARAMETRIZABLE == this._doEntidad);
            else
                this.Query = context.V_LPARAMETROS_CONFIG_LCON020;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
