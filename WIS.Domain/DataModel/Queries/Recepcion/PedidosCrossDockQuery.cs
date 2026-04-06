using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class PedidosCrossDockQuery : QueryObject<V_REC200_PEDIDOS_CROSS_DOCK, WISDB>
    {
        protected readonly Agenda _agenda;
        protected readonly string _pedido;
        protected readonly CrossDockingSeleccionTipo _tipoSeleccion;

        public PedidosCrossDockQuery(Agenda agenda, CrossDockingSeleccionTipo tipoSeleccion)
        {
            this._agenda = agenda;
            this._tipoSeleccion = tipoSeleccion;
        }

        public PedidosCrossDockQuery(Agenda agenda, CrossDockingSeleccionTipo tipoSeleccion, string pedido)
        {
            this._agenda = agenda;
            this._pedido = pedido;
            this._tipoSeleccion = tipoSeleccion;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC200_PEDIDOS_CROSS_DOCK.AsNoTracking().Where(d => d.CD_EMPRESA == this._agenda.IdEmpresa && d.NU_AGENDA == this._agenda.Id && (d.NU_PREDIO == this._agenda.Predio || d.NU_PREDIO == null));

            if (this._tipoSeleccion == CrossDockingSeleccionTipo.OrdenDeCompra)
                this.Query = this.Query.Where(d => d.NU_PEDIDO == this._agenda.NumeroDocumento);

            if (!string.IsNullOrEmpty(this._pedido))
                this.Query = this.Query.Where(d => d.NU_PEDIDO == this._pedido);
        }

        public virtual IEnumerable<Pedido> GetPedidos()
        {
            return this.Query.Select(d => new Pedido
            {
                Id = d.NU_PEDIDO,
                Cliente = d.CD_CLIENTE,
                Empresa = d.CD_EMPRESA
            });
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
