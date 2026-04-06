using System.Xml;
using System.Xml.Serialization;

namespace WIS.XmlData.WInterface.Helpers
{
    public class BaseSerializableXml
    {
        public static string ToXml(object obj)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(obj.GetType());
            var settings = new XmlWriterSettings();

            settings.Indent = false;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, obj, emptyNamepsaces);
                return stream.ToString();
            }
        }

        public static T LoadFromXMLString<T>(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
