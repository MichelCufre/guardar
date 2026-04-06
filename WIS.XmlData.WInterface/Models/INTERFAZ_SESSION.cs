namespace WIS.XmlData.WInterface.Models
{
    public class INTERFAZ_SESSION
    {

        [System.Runtime.Serialization.DataMember(IsRequired = true)]
        public string ID_USER { get; set; }
        public string PASSWORD { get; set; }
        public string SESION { get; set; }
    }
}
