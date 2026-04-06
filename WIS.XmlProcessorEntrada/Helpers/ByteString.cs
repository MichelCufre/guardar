using System.Text;

namespace WIS.XmlProcessorEntrada.Helpers
{
    public class ByteString
    {
        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
