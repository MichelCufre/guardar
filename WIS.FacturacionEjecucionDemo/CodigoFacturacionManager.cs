using System;
using System.Data.Common;
using WIS.Domain.Facturacion;
using WIS.Domain.Services.Interfaces;

namespace WIS.FacturacionEjecucionDemo
{
    public class CodigoFacturacionManager : ICodigoFacturacionManager
    {
        public object Calcular(DbConnection connection, DbTransaction tran, FacturacionEjecEmpProceso proceso, DateTime? fechaDesde, DateTime fechaHasta, string cuentaContable, long nuTransaccion)
        {
            string result = string.Empty;
            switch (proceso.CodigoFacturacion)
            {
                case "WCT001":
                    result = "Se proceso el código custom WCT001.";
                    break;
                default:
                    result = "No hay código de facturación a procesar";
                    break;
            }

            return result;
        }
    }
}
