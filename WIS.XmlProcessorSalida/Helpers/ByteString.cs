using System.Text;

namespace WIS.XmlProcessorSalida.Helpers
{
    public class ByteString
    {
        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static List<byte[]> SplitByteArray(string longString, int num_bytes = 1000000) //por defecto 1MB https://es.wikipedia.org/wiki/Byte
        {
            var source = GetBytes(longString);
            var largo = source.Length;
            var result = new List<byte[]>();

            if (num_bytes > largo) 
                num_bytes = largo;

            for (int i = 0; i < source.Length; i += num_bytes)
            {
                int take = num_bytes;

                if (largo - i < take)
                {
                    take = largo - i;
                }

                byte[] buffer = new byte[take];

                Buffer.BlockCopy(source, i, buffer, 0, take);

                result.Add(buffer);
            }

            return result;
        }
    }
}
