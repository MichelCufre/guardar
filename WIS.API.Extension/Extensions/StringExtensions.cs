using System.Text.RegularExpressions;

namespace WIS.API.Extension.Extensions
{
    public static class StringExtensions
    {
        public static string NullableTrim(this string input, bool keepEmptyOrNull = false, string defaultIfEmptyOrNull = null)
        {
            if (input == null)
                return !keepEmptyOrNull ? defaultIfEmptyOrNull : null;

            if (input.Trim() == string.Empty)
                return !keepEmptyOrNull ? defaultIfEmptyOrNull : input.Trim();

            return input.Trim();
        }

        public static string RemoveNewLineAndTrim(this string input)
        {
            if (input == null)
                return null;

            input = Regex.Replace(input, @"\t|\n|\r", "");
            input = input.Trim();

            return input;
        }
    }
}
