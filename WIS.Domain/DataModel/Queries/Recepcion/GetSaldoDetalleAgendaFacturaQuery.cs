using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    [Obsolete("No utilizar, no aplica el manejo de saldos")]
    public class GetSaldoDetalleAgendaFacturaQuery : QueryObject<V_SALDO_ORDEN_COMPRA_FAC, WISDB>
    {

        protected readonly List<int> _referencias;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _identificador;

        [Obsolete]
        public GetSaldoDetalleAgendaFacturaQuery(List<int> referencias, int idEmpresa, string codigoProducto, string identificador)
        {
            this._referencias = referencias;
            this._empresa = idEmpresa;
            this._producto = codigoProducto;
            this._identificador = identificador;

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SALDO_ORDEN_COMPRA_FAC.AsNoTracking().Where(w =>
                     _referencias.Contains(w.NU_RECEPCION_REFERENCIA)
                  && w.CD_EMPRESA == _empresa
                  && w.CD_PRODUTO == _producto
                  && (w.NU_IDENTIFICADOR == _identificador));
        }

        public virtual decimal GetSaldo()
        {
            return this.Query.ToList().Sum(w => w.QT_SALDO) ?? 0;
        }

    }
}
