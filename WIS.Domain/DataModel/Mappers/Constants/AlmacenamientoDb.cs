using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class AlmacenamientoDb
    {
        public const int LOGICA_UBICACIONES_VACIAS = 1;
        public const int LOGICA_REMONTE_UBICACIONES = 2;
        public const int LOGICA_REABASTECIMIENTOS = 3;

        public const string ESTADO_APROBADO = "A";
        public const string ESTADO_PENDIENTE = "P";
        public const string ESTADO_RECHAZADO = "R";

        public const string TIPO_OPERATIVA_CLASIFICACION = "CLASIF";
        public const string TIPO_OPERATIVA_FRACCIONADO = "ALMFRA";
        public const string TIPO_OPERATIVA_TRANSFERENCIA = "TRANSFEREN";
        public const string TIPO_OPERATIVA_PRODUCCION = "SALPROD";

        public const string TP_DOCUMENTO_AGENDA_DEVOLUCION = "9";

        public const string TIPO_ENTIDAD_CLASE = "CLASE";
        public const string TIPO_ENTIDAD_GRUPO = "GRUPO";
        public const string TIPO_ENTIDAD_PRODUCTO = "PRODUTO";
    }
}
