using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionEjecucion
    {
        public int NumeroEjecucion { get; set; }          //NU_EJECUCION
        public string Nombre { get; set; }                //NM_EJECUCION
        public string EjecucionPorHora { get; set; }      //FL_EJEC_POR_HORA
        public DateTime? FechaDesde { get; set; }         //DT_DESDE
        public DateTime? FechaHasta { get; set; }         //DT_HASTA
        public DateTime? CorteQuincena { get; set; }      //DT_CORTE_QUINCENA
        public DateTime? FechaIngresado { get; set; }     //DT_ADDROW
        public DateTime? FechaProgramacion { get; set; }  //DT_PROGRAMACION
        public DateTime? FechaEjecucion { get; set; }     //DT_EJECUCION
        public DateTime? FechaAprobacion { get; set; }    //DT_APROBACION
        public DateTime? FechaEnviada { get; set; }       //DT_ENVIADA
        public DateTime? FechaAnulacion { get; set; }     //DT_ANULACION
        public DateTime? FechaModificacion { get; set; }  //DT_UPDROW
        public int? CodigoFuncEjecucion { get; set; }     //CD_FUNC_EJECUCION
        public int? CodigoFuncProgramacion { get; set; }  //CD_FUNC_PROGRAMACION
        public int? CodigoFuncAprobacion { get; set; }    //CD_FUNC_APROBACION
        public int? CodigoFuncAnulacion { get; set; }     //CD_FUNC_ANULACION
        public int? CodigoFuncEnviada { get; set; }       //CD_FUNC_ENVIADA
        public short? CodigoSituacion { get; set; }       //CD_SITUACAO
    }
}
