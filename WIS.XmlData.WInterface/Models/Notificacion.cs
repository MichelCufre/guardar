using System.Xml.Serialization;
using WIS.XmlData.WInterface.Helpers;

namespace WIS.XmlData.WInterface.Models
{
    [XmlType("NOTIFICACIONES")]
    public class NOTIFICACIONES : List<NOTIFICACION>
    {
        public static T LoadFromXMLString<T>(string xml)
        {
            return BaseSerializableXml.LoadFromXMLString<T>(xml);
        }

        public List<NOTIFICACION> Items()
        {
            return new List<NOTIFICACION>();
        }
    }

    public class NOTIFICACION
    {
        public string ID_NOTIFICACION { get; set; }
        public string VL_CATEGORIA { get; set; }
        public string VL_NIVEL { get; set; }
        public string VL_ESTADO { get; set; }
        public string DS_MENSAJE { get; set; }
        public string VL_SERIALIZADO { get; set; }
    }
}
