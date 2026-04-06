using System.Xml.Serialization;

namespace WIS.XmlData.WInterface.Models
{
    public enum EResultado { OK, ERROR, PENDIENTE }

    [XmlRoot(DataType = "RESPUESTA_INTERFAZ", ElementName = "RESPUESTA_INTERFAZ")]
    public class RESPUESTA_INTERFAZ
    {
        public RESPUESTA_INTERFAZ()
        {
            NU_INTERFAZ_EJECUCION = null;
            ERRORES = new List<ERROR>();
            EJECUCIONES = new List<EJECUCION>();
            RESULTADO = "OK";
            TOKEN = "";
            MENSAJE = "";
            PAQUETE = null;
            TOTAL_PAQUETES = null;
            DATA = string.Empty;
            CD_INTERFAZ_EXTERNA = null;
        }

        public RESPUESTA_INTERFAZ(EResultado resultado)
        {
            NU_INTERFAZ_EJECUCION = null;
            ERRORES = new List<ERROR>();
            EJECUCIONES = new List<EJECUCION>();
            RESULTADO = resultado.ToString();
            TOKEN = "";
            MENSAJE = "";
            PAQUETE = null;
            TOTAL_PAQUETES = null;
        }

        public string RESULTADO { get; set; }
        public string NU_INTERFAZ_EJECUCION { get; set; }
        public string TOKEN { get; set; }
        public string MENSAJE { get; set; }
        public List<EJECUCION> EJECUCIONES { get; set; }
        public List<ERROR> ERRORES { get; set; }
        public string PAQUETE { get; set; }
        public string TOTAL_PAQUETES { get; set; }
        public string CD_INTERFAZ_EXTERNA { get; set; }
        public string DATA { get; set; }

        public void SetStatus(EResultado value)
        {
            RESULTADO = Enum.GetName(typeof(EResultado), value);
        }

        public void AddError(XDErrorCodigo err)
        {
            ERROR error = new ERROR();
            error.CD_ERROR = err.Codigo;
            error.DS_ERROR = err.Descripcion;
            error.NU_ERROR = ERRORES.Count + 1;
            ERRORES.Add(error);
        }

        public void AddErrores(List<XDErrorCodigo> list)
        {
            if (list != null)
            {
                foreach (XDErrorCodigo err in list)
                {
                    AddError(err);
                }
            }
        }
    }
}
