using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class FacturaQuery : QueryObject<V_REC500_FACTURAS, WISDB>
    {
        protected readonly int? _factura;
        protected readonly bool _todos;
        protected readonly int? _nuAgenda;
        protected readonly string _predio;
        protected readonly string _cdCliente;

        public FacturaQuery(int? factura = null, int? nuAgenda = null, string predio = null, string cdCliente = null, bool todos = false)
        {
            this._factura = factura;
            this._nuAgenda = nuAgenda;
            this._predio = predio;
            this._cdCliente = cdCliente;
            this._todos= todos;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC500_FACTURAS
                .AsNoTracking()
                .Where(x => _todos ? true :
                    ((!string.IsNullOrEmpty(this._cdCliente) ? x.CD_CLIENTE == this._cdCliente : true) &&
                    (!string.IsNullOrEmpty(this._predio) ? x.NU_PREDIO == this._predio : true) &&
                    (this._nuAgenda.HasValue ? (x.NU_AGENDA == this._nuAgenda) : (x.NU_AGENDA == null && x.ND_ESTADO == EstadoFacturaDb.Pendiente)) &&
                    (this._factura.HasValue ? x.NU_RECEPCION_FACTURA == this._factura : true)));
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<AsociarAgenda> GetFacturasUnidad()
        {
            return this.Query.Select(d => new AsociarAgenda
            {
                IdFactura = d.NU_RECEPCION_FACTURA,
                Cliente = d.CD_CLIENTE,
                Empresa = d.CD_EMPRESA
            }).ToList();
        }

    }
}

