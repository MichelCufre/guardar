using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PedidosPendientesQuery : QueryObject<V_PEDIDOS_PENDIENTES, WISDB>
    {

        protected readonly string _codigoInternoAgente;
        protected readonly int _idEmpresa;
        protected readonly string _idPredio;
        protected readonly short _idRota;

        public PedidosPendientesQuery(int idEmpresa, string codigoInternoAgente, string idPredio, short idRota)
        {
            this._idEmpresa = idEmpresa;
            this._codigoInternoAgente = codigoInternoAgente;
            this._idPredio = idPredio;
            this._idRota = idRota;

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PEDIDOS_PENDIENTES.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual bool AnyPedidosPendientes()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer la consulta");

            return this.Query.Any(s => s.CD_EMPRESA == this._idEmpresa
                                    && s.CD_CLIENTE == this._codigoInternoAgente
                                    && s.NU_PREDIO == this._idPredio
                                    && s.CD_ROTA == this._idRota);
        }

    }

}
