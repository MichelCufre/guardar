using System.Text;

namespace WIS.XmlData.WInterface.Helpers
{
    public class ByteString
    {
        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
