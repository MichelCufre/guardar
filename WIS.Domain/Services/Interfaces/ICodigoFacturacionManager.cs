using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using WIS.Domain.Facturacion;

namespace WIS.Domain.Services.Interfaces
{
    public interface ICodigoFacturacionManager
    {
        public object Calcular(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, DateTime? fechaDesde, DateTime fechaHasta, string cuentaContable, long nuTransaccion);
    }
}
