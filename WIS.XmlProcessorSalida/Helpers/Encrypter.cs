namespace WIS.XmlProcessorSalida.Helpers
{
    public static class Encrypter
    {
        public static string EncodeBase64(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
