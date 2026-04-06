using System.ComponentModel;
using System.Reflection;

namespace WIS.XmlData.WInterface.Models
{
    public class XDErrorCodigo
    {
        private string _codigo;
        private string _descripcion;

        public string Codigo { get { return _codigo; } }
        public string Descripcion { get { return _descripcion; } }

        public XDErrorCodigo(XDCDError error, string extradesc = null)
        {
            string value = GetEnumDescription(error);
            string[] info = value.Split(new string[] { "::" }, StringSplitOptions.None);
            _codigo = info[0];
            _descripcion = info[1];
            if (extradesc != null && _descripcion.IndexOf("{0}") > -1)
            {
                _descripcion = string.Format(_descripcion, extradesc);
            }
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
