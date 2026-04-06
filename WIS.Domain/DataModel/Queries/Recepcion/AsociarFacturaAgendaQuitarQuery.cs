using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class AsociarFacturaAgendaQuitarQuery : QueryObject<V_REC500_FACTURA_AGENDA, WISDB>
    {
        protected readonly int _idAgenda;

        public AsociarFacturaAgendaQuitarQuery(int idAgenda)
        {
            this._idAgenda = idAgenda;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC500_FACTURA_AGENDA.Where(d => d.NU_AGENDA == this._idAgenda);
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
