using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class REC270Grid1Query : QueryObject<V_ETIQUETA_LOTE_WREC270, WISDB>
    {
        protected int NU_AGENDA;

        public REC270Grid1Query(int agenda = -1)
        {
            this.NU_AGENDA = agenda;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ETIQUETA_LOTE_WREC270.AsNoTracking();

            this.Query = this.Query.Where(el => el.CD_CLIENTE != null && el.CD_SITUACAO == SituacionDb.TransferidoAContenedores && el.NU_AGENDA == NU_AGENDA);

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
