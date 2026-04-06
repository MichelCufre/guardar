using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class PedidosIncluidosCrossDockQuery : QueryObject<V_REC200_SELECCION_CROSS_DOCK, WISDB>
    {
        protected readonly int _empresa;
        protected readonly int _agenda;
        protected readonly string _predio;

        public PedidosIncluidosCrossDockQuery(int empresa, int agenda, string predio)
        {
            this._empresa = empresa;
            this._agenda = agenda;
            this._predio = predio;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC200_SELECCION_CROSS_DOCK.AsNoTracking().Where(d => d.CD_EMPRESA == this._empresa && d.NU_AGENDA == this._agenda && (d.NU_PREDIO == this._predio || d.NU_PREDIO == null));
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<CrossDockingPedidoSelection> GetDetallesCross()
        {
            return this.Query.Select(d => new CrossDockingPedidoSelection
            {
                Pedido = d.NU_PEDIDO,
                Empresa = d.CD_EMPRESA,
                Cliente = d.CD_CLIENTE,
            }).ToList();
        }
    }
}
