using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PedidoReabastecimientoQueryObject : QueryObject<V_REAB_PRE680, WISDB>
    {
        protected readonly FiltrosReabasteciminento _filtros;

        public PedidoReabastecimientoQueryObject(FiltrosReabasteciminento filtro)
        {
            _filtros = filtro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REAB_PRE680.AsNoTracking();
            int empresa = -1;
            if (!string.IsNullOrEmpty(_filtros.CD_EMPRESA))
            {
                empresa = int.Parse(_filtros.CD_EMPRESA);
            }
            if (empresa >= 0)
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == empresa);
            }

            if (!string.IsNullOrEmpty(_filtros.NU_PREDIO) && !_filtros.NU_PREDIO.Equals(GeneralDb.PredioSinDefinir))
            {
                string predio = _filtros.NU_PREDIO;
                this.Query = this.Query.Where(x => x.NU_PREDIO_NECESIDAD == predio);
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
