using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class FacturacionDb
    {
        public const string FacturaProductoCompra = "FACPROCOM";

        //Estados
        public const string ESTADO_HAB = "HAB";
        public const string ESTADO_CAN = "CAN";
        public const string ESTADO_FAC = "FAC";

        #region TiposDeCalculos
        public const string TIPO_DE_CALCULO_MANUAL = "M";
        public const string TIPO_DE_CALCULO_CALCULADO = "C";
        public const string TIPO_DE_CALCULO_TAREA = "T";
        public const string TIPO_DE_CALCULO_PROGRAMABLE = "P";
        public const string TIPO_DE_CALCULO_AMIGABLE = "A";

        public virtual List<string> getTiposCalculo()
        {
            List<string> colTiposCalculos = new List<string>();

            colTiposCalculos.Add(TIPO_DE_CALCULO_MANUAL);
            colTiposCalculos.Add(TIPO_DE_CALCULO_CALCULADO);
            colTiposCalculos.Add(TIPO_DE_CALCULO_TAREA);
            colTiposCalculos.Add(TIPO_DE_CALCULO_PROGRAMABLE);
            colTiposCalculos.Add(TIPO_DE_CALCULO_AMIGABLE);

            return colTiposCalculos;
        }

        #endregion

        #region CodigosFacturacion

        public const string CD_FACT_WST001 = "WST001";
        public const string CD_FACT_WST002 = "WST002";
        public const string CD_FACT_WST003 = "WST003";
        public const string CD_FACT_WST004 = "WST004";
        public const string CD_FACT_WST005 = "WST005";
        #endregion

        #region TiposDeCalculosTareasAmigables
        //TRABAJOS MANUALES
        public static readonly string CALCULO_M = "MANUAL";
        public static readonly string CALCULO_C = "CALCULADO";
        public static readonly string CALCULO_T = "TAREA";
        public static readonly string CALCULO_P = "PROGRAMABLE";
        public static readonly string CALCULO_A = "AMIGABLE";

        public static readonly string CIERRE_PROCESO_VALOR_DEFAULT = "N";

        //ORDEN TAREA 

        public const short NU_SECUENCIA_TAREA_VALOR_DEFAULT = 1;
        public static readonly string FL_RESUELTA_VALOR_DEFAULT = "N";
        public static readonly string FL_RESUELTA_VALOR_RESULETA = "S";

        //ORDEN FUNCIONARIO

        public const short NU_ORDEN_TAREA_VALOR_DEFAULT = 0;
        public const short QT_HORAS_VALOR_DEFAULT = 0;
        public const short QT_HORAS_EXTRA_VALOR_DEFAULT = 0;

        #endregion

        public const short EMP_PROCESO_ERROR = 2;
        public const string EJECUCION_HORA = "N";
    }
}
