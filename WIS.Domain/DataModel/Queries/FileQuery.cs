using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class FileQuery : QueryObject<V_ARCHIVOS, WISDB>
    {
        protected string _tpEntidad;
        protected string _cdEntidad;
        public FileQuery(string tpEntidad = null, string cdEntidad = null)
        {
            this._tpEntidad = tpEntidad;
            this._cdEntidad = cdEntidad;
        }

        public override void BuildQuery(WISDB context)
        {
            if (!string.IsNullOrEmpty(this._tpEntidad) && !string.IsNullOrEmpty(this._cdEntidad))
                this.Query = context.V_ARCHIVOS.Where(w => w.TP_ENTIDAD == _tpEntidad && w.CD_ENTIDAD == _cdEntidad);
            else
                this.Query = context.V_ARCHIVOS;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
