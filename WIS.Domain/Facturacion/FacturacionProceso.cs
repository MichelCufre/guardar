using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionProceso
    {
        public string CodigoProceso { get; set; }          //CD_PROCESO
        public short? CodigoSituacionError{ get; set; }     //CD_SITUACAO_ERROR
        public string CodigoFacturacion { get; set; }       //CD_FACTURACION
        public string NombreProcedimiento { get; set; }     //NM_PROCEDIMIENTO
        public string DescripcionProceso  { get; set; }     //DS_PROCESO
        public string NumeroComponente { get; set; }        //NU_COMPONENTE
        public string NumeroCuentaContable { get; set; }    //NU_CUENTA_CONTABLE
        public string TipoProceso { get; set; }             //TP_PROCESO
        public string EjecucionPorHora { get; set; }        //FL_EJEC_POR_HORA
    }
}
