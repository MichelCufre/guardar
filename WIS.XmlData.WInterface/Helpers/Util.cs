namespace WIS.XmlData.WInterface.Helpers
{
    public static class Util
    {
        public static bool IsNullOrEmpty(this string val)
        {
            if (val == null)
                return true;

            if (val.Equals(string.Empty))
                return true;

            return false;
        }
    }
}
