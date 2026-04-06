using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Evento
{
    public class VersionesArchivosQuery : QueryObject<V_ARCHIVO_VERSION, WISDB>
    {
        protected long? _nuArchivoAdjunto;

        public VersionesArchivosQuery(long? nuArchivoAdjunto)
        {
            this._nuArchivoAdjunto = nuArchivoAdjunto;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ARCHIVO_VERSION.AsNoTracking();

            if (_nuArchivoAdjunto != null)
            {
                this.Query = this.Query.Where(w => w.NU_ARCHIVO_ADJUNTO == _nuArchivoAdjunto);
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
