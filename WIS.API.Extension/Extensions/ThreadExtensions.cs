using System.Globalization;
using System.Threading;

namespace WIS.API.Extension.Extensions
{
    public static class ThreadExtensions
    {
        public static void SetCulture(this Thread thread, string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
                return;

            thread.SetCulture(new CultureInfo(culture.Trim()));
        }

        public static void SetCulture(this Thread thread, CultureInfo culture)
        {
            if (culture == null)
                return;

            thread.CurrentCulture = culture;
            thread.CurrentUICulture = culture;
        }
    }
}
