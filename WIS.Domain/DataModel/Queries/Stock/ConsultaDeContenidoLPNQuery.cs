using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaDeContenidoLPNQuery : QueryObject<V_STO720_LPN_LINEAS, WISDB>
    {
        protected long? _numeroLPN;
        protected bool _lpnsActivos;

        public ConsultaDeContenidoLPNQuery(long? numeroLPN = null, bool lpnsActivos = true)
        {
            _numeroLPN = numeroLPN;
            _lpnsActivos = lpnsActivos;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO720_LPN_LINEAS
                .AsNoTracking()
                .Where(l => (_numeroLPN.HasValue ? l.NU_LPN == _numeroLPN : true)
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