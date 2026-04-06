using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class INV412GridQuery : QueryObject<V_INV412_DET_CONTEO, WISDB>
    {
        protected decimal? _nuInventario;
        protected bool _showOnlyPending;

        public INV412GridQuery(decimal? nuInventario, bool showOnlyPending)
        {
            this._nuInventario = nuInventario;
            this._showOnlyPending = showOnlyPending;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INV412_DET_CONTEO
                .Where(i => (_nuInventario.HasValue ? i.NU_INVENTARIO == _nuInventario : true)
                    && (_showOnlyPending ? i.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR : true));
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
