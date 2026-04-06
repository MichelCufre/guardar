using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class TraspasoEmpresasDetallePedidosQuery : QueryObject<V_STO820_DETALLE_PEDIDOS, WISDB>
    {
        protected readonly IUnitOfWork _uow;
        protected readonly long _nuTraspaso;
        protected readonly int _nuPreparacion;

        public TraspasoEmpresasDetallePedidosQuery(IUnitOfWork uow, long nuTraspaso, int nuPreparacion)
        {
            this._uow = uow;
            this._nuTraspaso = nuTraspaso;
            this._nuPreparacion = nuPreparacion;
        }

        public override void BuildQuery(WISDB context)
        {
            var registros = context.V_STO820_DETALLE_PEDIDOS.AsNoTracking().Where(x => x.NU_TRASPASO == _nuTraspaso);

            if (registros.Any() || _nuPreparacion == -1)
            {
                this.Query = registros;
            }
            else
            {
                var traspaso = _uow.TraspasoEmpresasRepository.GetTraspaso(_nuTraspaso);
                var cdEmpresaDestino = traspaso.EmpresaDestino;

                this.Query = context.T_DET_PICKING
                  .Where(dp => dp.NU_PREPARACION == _nuPreparacion)
                  .Join(context.T_PICKING,
                      dp => dp.NU_PREPARACION,
                      p => p.NU_PREPARACION,
                      (dp, p) => dp)
                  .Join(context.T_PEDIDO_SAIDA,
                      dp => new { dp.NU_PEDIDO, dp.CD_CLIENTE, dp.CD_EMPRESA },
                      pd => new { pd.NU_PEDIDO, pd.CD_CLIENTE, pd.CD_EMPRESA },
                      (dp, pd) => new { dp.NU_PEDIDO, dp.CD_CLIENTE, dp.CD_EMPRESA, pd.TP_PEDIDO, pd.TP_EXPEDICION })
                  .Distinct()
                  .Select(x => new V_STO820_DETALLE_PEDIDOS
                  {
                      NU_TRASPASO = _nuTraspaso,
                      NU_PEDIDO = x.NU_PEDIDO,
                      CD_CLIENTE = x.CD_CLIENTE,
                      CD_EMPRESA = x.CD_EMPRESA,
                      NU_PEDIDO_DESTINO = null,
                      CD_CLIENTE_DESTINO = null,
                      TP_PEDIDO_DESTINO = "",
                      TP_EXPEDICION_DESTINO = "",
                      CD_EMPRESA_DESTINO = cdEmpresaDestino,
                      TP_EXPEDICION = x.TP_EXPEDICION,
                      TP_PEDIDO = x.TP_PEDIDO,
                  });
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
