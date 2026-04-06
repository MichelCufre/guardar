using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Configuracion
{
    public class ConfiguracionReporteQuery : QueryObject<V_COF070_REPORTE, WISDB>
    {
        protected readonly string _cdCamion;
        protected readonly string _nroAgenda;

        public ConfiguracionReporteQuery(string cdCamion, string nroAgenda)
        {
            if (!string.IsNullOrEmpty(cdCamion))
                this._cdCamion = cdCamion;
            else
                this._nroAgenda = nroAgenda;
        }

        public ConfiguracionReporteQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_COF070_REPORTE.AsNoTracking();

            if (!string.IsNullOrEmpty(this._cdCamion))
                this.Query = this.Query.Where(q => q.NM_TABLA == "T_CAMION" && q.CD_CLAVE.StartsWith(this._cdCamion));

            if (!string.IsNullOrEmpty(this._nroAgenda))
                this.Query = this.Query.Where(q => q.NM_TABLA == "T_AGENDA" && q.CD_CLAVE.StartsWith(this._nroAgenda));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
