using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionEjecEmpProceso
    {
        public int NumeroEjecucion { get; set; }                  //NU_EJECUCION
        public string Nombre { get; set; }                        //NM_EJECUCION
        public string EjecucionPorHora { get; set; }              //FL_EJEC_POR_HORA_EJEC
        public DateTime? FechaDesdeEjecucion { get; set; }        //DT_DESDE_EJECUCION
        public DateTime? FechaHastaEjecucion { get; set; }        //DT_HASTA_EJECUCION
        public DateTime? CorteQuincena { get; set; }              //DT_CORTE_QUINCENA
        public DateTime? FechaIngresado { get; set; }             //DT_ADDROW
        public DateTime? FechaProgramacion { get; set; }          //DT_PROGRAMACION
        public DateTime? FechaEjecucion { get; set; }             //DT_EJECUCION
        public DateTime? FechaAprobacion { get; set; }            //DT_APROBACION
        public DateTime? FechaEnviada { get; set; }               //DT_ENVIADA
        public DateTime? FechaAnulacion { get; set; }             //DT_ANULACION
        public int? CodigoFuncEjecucion { get; set; }             //CD_FUNC_EJECUCION
        public int? CodigoFuncProgramacion { get; set; }          //CD_FUNC_PROGRAMACION
        public int? CodigoFuncAprobacion { get; set; }            //CD_FUNC_APROBACION
        public int? CodigoFuncAnulacion { get; set; }             //CD_FUNC_ANULACION
        public int? CodigoFuncEnviada { get; set; }               //CD_FUNC_ENVIADA
        public short? SituacionEjecucion { get; set; }            //CD_SITUACAO_EJECUCION

        public int CodigoEmpresa { get; set; }                    //CD_EMPRESA
        public string CodigoProceso { get; set; }                 //CD_PROCESO
        public string Estado { get; set; }                        //ID_ESTADO
        public short? SituacionEjecEmp{ get; set; }               //CD_SITUACAO_EJEC_EMP
        public DateTime? FechaDesdeEjecEmp { get; set; }          //DT_DESDE_EJEC_EMP
        public DateTime? FechaHastaEjecEmp { get; set; }          //DT_HASTA_EJEC_EMP

        public string CdProgreso { get; set; }                //CD_PROCESO
        public short? SituacionErrorProceso { get; set; }         //CD_SITUACAO_ERROR_PROCESO
        public string CodigoFacturacion { get; set; }             //CD_FACTURACION
        public string NombreProcedimiento { get; set; }           //NM_PROCEDIMIENTO
        public string DescripcionProceso { get; set; }            //DS_PROCESO
        public string NumeroComponente { get; set; }              //NU_COMPONENTE
        public string NumeroCuentaContable { get; set; }          //NU_CUENTA_CONTABLE
        public string TipoProceso { get; set; }                   //TP_PROCESO
        public string EjecucionPorHoraProceso { get; set; }       //FL_EJEC_POR_HORA_EJEC_EMP
    }
}
