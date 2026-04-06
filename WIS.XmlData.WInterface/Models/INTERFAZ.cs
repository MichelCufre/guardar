namespace WIS.XmlData.WInterface.Models
{
    public class INTERFAZ
    {
        public INTERFAZ()
        {
            NU_INTERFAZ_EJECUCION = null;
            CD_EMPRESA = null;
            CD_INTERFAZ_EXTERNA = null;
            NU_PAQUETE = null;
            TOTAL_PAQUETES = null;
            DATA = string.Empty;
            FINALIZA_EJECUCION = string.Empty;
            TOKEN = string.Empty;
            DS_REFERENCIA = string.Empty;
            ERRORES = new List<ERROR>();
        }

        public string NU_INTERFAZ_EJECUCION { get; set; }
        public string CD_EMPRESA { get; set; }
        public string CD_INTERFAZ_EXTERNA { get; set; }
        public string NU_PAQUETE { get; set; }
        public string TOTAL_PAQUETES { get; set; }
        public string DS_REFERENCIA { get; set; }
        public string TOKEN { get; set; }
        public string DATA { get; set; }
        public string FINALIZA_EJECUCION { get; set; }
        public List<ERROR> ERRORES { get; set; }
    }
}
