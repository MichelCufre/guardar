using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaDeLpnQuery : QueryObject<V_STO700_LPN, WISDB>
    {
        protected readonly int? _agenda;
        protected readonly bool _lpnsActivos;

        public ConsultaDeLpnQuery(int? agenda = null, bool lpnsActivos = true)
        {
            _agenda = agenda;
            _lpnsActivos = lpnsActivos;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO700_LPN
                .Where(l => (_agenda != null ? l.NU_AGENDA == _agenda : true)
                    && (_lpnsActivos ? l.ID_ESTADO != EstadosLPN.Finalizado : true));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}