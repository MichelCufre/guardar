namespace WIS.XmlData.WInterface.Models
{
    public class EJECUCION
    {
        public EJECUCION()
        {

        }

        public EJECUCION(long value)
        {
            NU_INTERFAZ_EJECUCION = value;
            CD_INTERFAZ_EXTERNA = -1;
        }

        public long NU_INTERFAZ_EJECUCION { get; set; }
        public int CD_INTERFAZ_EXTERNA { get; set; }
    }
}
