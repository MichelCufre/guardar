using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class CReporte
    {
        //Estado
        public const string Pendiente = "SREPPEND";
        public const string Procesado = "SREPPROC";
        public const string Anulado = "SREPANUL";
        public const string EnviadoImpresion = "SREPIMPR";
        public const string Error = "SREPERRO";
        public const string ErrorImpresion = "SREPERIM";
        public const string PendienteReprocesar = "SREPPENR";
        public const string PendienteReimprimir = "SREPPERI";
        public const string Reprocesado = "SREPRPRO";

        //Tipo
        public const string TipoReporteRecepcion = "TPREPREC";
        public const string TipoReporteExpedicion = "TPREPEXP";

        //Tabla
        public const string TablaReporteAgenda = "T_AGENDA";
        public const string TablaReporteCamion = "T_CAMION";

        //Recepcion
        public const string NOTA_DEVOLUCION = "NOTA_DEVOLUCION";
        public const string CONFIRMACION_RECEPCION = "CONFIRMACION_RECEPCION";

        //Expedicion
        public const string PACKING_LIST = "PACKING_LIST";
        public const string CONTENEDORES_CAMION = "CONTENEDORES_CAMION";
        public const string CONTROL_CAMBIO = "CONTROL_CAMBIO";
        public const string PACKING_LIST_SIN_LPN = "PACKING_LIST_SIN_LPN";

        //GAPS
        public const string GAP = "______________________________";
    }
}
