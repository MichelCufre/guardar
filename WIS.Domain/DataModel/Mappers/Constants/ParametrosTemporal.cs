using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class ParametrosTemporal //TODO: Eliminar, hablar con gonzalo sobre como trabajar con parametros
    {
        public static string CD_PARAMETRO_TP_INVENT = "TIPO_INVENTARIO";
        public static string CD_PARAMETRO_CIERRE_CONTEO = "CIERRE_CONTEO";
        public static string CD_PARAMETRO_EMPRESA_INV = "EMPRESA_INV";
        public static string CD_PARAMETRO_FL_CONTROLAR_VENCIMINENTO = "FL_CONTROLAR_VENCIMIENTO";
        public static string CD_PARAMETRO_FL_ACTUALIZAR_CONTEO_FIN_AUTO = "FL_ACTUALIZAR_CONTEO_FIN_AUTO";
        public static string CD_PARAMETRO_FL_BLOQ_USR_CONTEO_SUCESIVO = "FL_BLOQ_USR_CONTEO_SUCESIVO";
        public static string CD_PARAMETRO_FL_MODIFICAR_STOCK_EN_DIF = "FL_MODIFICAR_STOCK_EN_DIF";
        public static string CD_PARAMETRO_FL_PERMITE_INGRESAR_MOTIVO = "FL_PERMITE_INGRESAR_MOTIVO";


        #region LPRE650

        public static string CD_PARAMETRO_LPRE650_ZONAS = "LPRE650_ZONAS";
        public static string CD_PARAMETRO_LPRE650_VENTANAS_HORARIAS = "LPRE650_VENTANAS_HORARIAS";
        public static string CD_PARAMETRO_LPRE650_TIEMPO_ENTREGA = "LPRE650_TIEMPO_ENTREGA";
        public static string CD_PARAMETRO_LPRE650_PRE_PED_COMPLETO = "LPRE650_PRE_PED_COMPLETO";

        #endregion

        public static string LPN_RECEPCION_PREFIJO_ETIQUETA = "LPN_RECEPCION_PREFIJO_ETIQUETA";
    }
}
