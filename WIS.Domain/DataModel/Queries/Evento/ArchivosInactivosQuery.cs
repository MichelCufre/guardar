using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Eventos.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Evento
{
    public class ArchivosInactivosQuery : QueryObject<V_ARCHIVO_ADJUNTO, WISDB>
    {
        public ArchivosInactivosQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ARCHIVO_ADJUNTO.AsNoTracking().Where(w => w.CD_SITUACAO == (short)EstadoArchivo.Inactivo);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
