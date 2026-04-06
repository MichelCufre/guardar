using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionEjecucionEmpresa
    {
        public int NumeroEjecucion { get; set; }      //NU_EJECUCION
        public int CodigoEmpresa { get; set; }        //CD_EMPRESA
        public string CodigoProceso { get; set; }     //CD_PROCESO
        public string Estado { get; set; }            //ID_ESTADO
        public short? CodigoSituacion { get; set; }   //CD_SITUACAO
        public DateTime? FechaDesde { get; set; }     //DT_DESDE
        public DateTime? FechaHasta { get; set; }     //DT_HASTA
        public DateTime? FechaModificacion { get; set; }     //DT_UPDROW
    }   
}
