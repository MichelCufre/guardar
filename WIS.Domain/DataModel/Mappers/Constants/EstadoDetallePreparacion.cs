using System.Collections.Generic;

namespace WIS.Domain.DataModel.Mappers.Constants
{
	public class EstadoDetallePreparacion
    {
        //Dominio ESTAD_PREP
        public const string ESTADO_ANULACION_EJECUTADA = "ANU_EJECUTADA";
        public const string ESTADO_ANULACION_EJECUTADA_DOC = "ANU_EJECUTADA_DOC";
        public const string ESTADO_ANULACION_PENDIENTE = "ANU_PENDIENTE";
        public const string ESTADO_ANULACION_DOC_ENV = "ANU_ENVIADO_DOC";
        public const string ESTADO_ANULACION_DOC_ERROR = "ANU_ERROR_DOC";
        public const string ESTADO_ANULACION_DOC_PEND = "ANU_PEND_ENV_DOC";
        public const string ESTADO_ANULACION_FIN_ERROR = "ANU_FIN_ERROR";

        public const string ESTADO_PREP_PENDIENTE = "ESTAD_PREP_PEND";
        public const string ESTADO_PREPARADO = "ESTAD_PREPARADO";
        public const string ESTADO_TRANSFERENCIA = "ESTAD_TRANSF";
        public const string ESTADO_PENDIENTE_AUTO = "ESTAD_PEND_AUT";
        public const string ESTAD_TRASPASADO  = "ESTAD_TRASPASADO";

        public static List<string> GetEstadosAnulacion()
        {
            return new List<string>()
            {
                ESTADO_ANULACION_EJECUTADA,
                ESTADO_ANULACION_EJECUTADA_DOC,
                ESTADO_ANULACION_PENDIENTE,
                ESTADO_ANULACION_DOC_ENV,
                ESTADO_ANULACION_DOC_ERROR,
                ESTADO_ANULACION_DOC_PEND,
                ESTADO_ANULACION_FIN_ERROR,
            };
        }

        public static List<string> GetEstadosAnulacionPendiente()
        {
            return new List<string>()
            {
                ESTADO_ANULACION_PENDIENTE,
                ESTADO_ANULACION_DOC_PEND,
            };
        }

        public static List<string> GetEstadosPickingPendiente()
        {
            return new List<string>()
            {
                ESTADO_PREP_PENDIENTE,
                ESTADO_TRANSFERENCIA,
                ESTADO_PENDIENTE_AUTO,
            };
        }
    }
}
