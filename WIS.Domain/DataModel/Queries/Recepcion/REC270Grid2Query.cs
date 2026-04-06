using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class REC270Grid2Query : QueryObject<V_ETIQUETA_PRE_SEP_WREC270, WISDB>
    {
        protected int NU_AGENDA;

        public REC270Grid2Query(int nuAgenda = -1)
        {
            this.NU_AGENDA = nuAgenda;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ETIQUETA_PRE_SEP_WREC270.AsNoTracking();
            this.Query = this.Query.Where(eps => (eps.MAX_SITUACAO == SituacionDb.AgendaConferidaSinDiferencia || eps.ID_CTRL_ACEPTADO == "S") && eps.NU_AGENDA == NU_AGENDA && eps.CD_SITUACAO != SituacionDb.TransferidoAContenedores);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
