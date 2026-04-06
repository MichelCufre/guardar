using WIS.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WIS.Persistence.Database;

namespace WIS.XmlProcessorEntrada.Helpers
{
    public enum ProcessorCDError
    {
        [Description("WIE-100")] //Error generico: msg…
        WIE_100,
        [Description("WIE-101")] //No hay datos para procesar asociados a esta ejecución
        WIE_101,
        [Description("WIE-102")] //Los datos recibidos por interfaz no corresponden con la estructura XML esperad
        WIE_102,
        [Description("WIE-103")] //Tipo de datos incorrecto - <campo>
        WIE_103,
        [Description("WIE-104")] //Ejecución pendiente de procesar con error de carga, la misma se finalizará.
        WIE_104
    }

    public class XmlProcessorError
    {
        public static int SaveErrores(WISDB context, long NU_INTERFAZ_EJECUCION, List<string> errores, ProcessorCDError cdError, int? nuRegistro = null, int? nuError = null)
        {
            int NU_ERROR = 1;
            string CD_ERROR = GetEnumDescription(cdError);

            if (nuError == null)
                NU_ERROR = 1;
            else
                NU_ERROR = nuError ?? 1;

            if (context.T_INTERFAZ_EJECUCION_ERROR.Any(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION))
                NU_ERROR = context.T_INTERFAZ_EJECUCION_ERROR.Where(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION).Max(y => y.NU_ERROR) + 1;
            if (context.T_INTERFAZ_EJECUCION_ERROR.Local.Any(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION))
                NU_ERROR = context.T_INTERFAZ_EJECUCION_ERROR.Local.Where(x => x.NU_INTERFAZ_EJECUCION == NU_INTERFAZ_EJECUCION).Max(y => y.NU_ERROR) + 1;
            
            foreach (string DS_ERROR in errores)
            {
                SaveError(context, NU_INTERFAZ_EJECUCION, NU_ERROR, CD_ERROR, DS_ERROR, nuRegistro);
                NU_ERROR++;
            }

            return NU_ERROR;
        }

        private static void SaveError(WISDB context, long NU_INTERFAZ_EJECUCION, int NU_ERROR, string CD_ERROR, string DS_ERROR, int? nuRegistro = null)
        {
            T_INTERFAZ_EJECUCION_ERROR error_db = new T_INTERFAZ_EJECUCION_ERROR();
            error_db.NU_INTERFAZ_EJECUCION = NU_INTERFAZ_EJECUCION;
            error_db.NU_REGISTRO = nuRegistro;
            error_db.NU_ERROR = NU_ERROR;
            error_db.CD_ERROR = CD_ERROR;
            error_db.DS_REFERENCIA = "WIS_PROCESSOR";
            error_db.DS_ERROR = DS_ERROR.Truncate(399);
            context.T_INTERFAZ_EJECUCION_ERROR.Add(error_db);
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
