using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// Trunca el valor de un string si este excede el maximo indicado
        /// </summary>
        /// <param name="value">Valor de string</param>
        /// <param name="maxLength">Largo maximo</param>
        /// /// <param name="enc">UTF8, ASCII, UNICODE</param>
        /// <returns>Valor de string truncado</returns>
        public static string Truncate(this string value, int maxLength, string enc = null)
        {
            if (value == null)
                return "";
            if (value.Length < maxLength)
                return value;
            else
                return value.Substring(0, maxLength);
        }

        /// <summary>
        /// Given a value, returns the substring occurring before the parameter search; string.Empty otherwise 
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <param name="search">The string to search for</param>
        /// <returns>The substring occurring before search in the string value</returns>
        public static string SubstringBefore(this string value, char search)
        {
            var index = value.IndexOf(search);
            return index != -1 ? value.Substring(0, index) : string.Empty;
        }

        /// <summary>
        /// Given a value, returns the substring occurring after the parameter search; string.Empty otherwise
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <param name="search">The string to search for</param>
        /// <returns>The substring occurring after search in the string value</returns>
        public static string SubstringAfter(this string value, char search)
        {
            var index = value.IndexOf(search);
            return index != -1 ? value.Substring(index + 1) : string.Empty;
        }

        public static T ToNumber<T>(this string value, string msg = "General_Sec0_Error_Error14")
        {
            try
            {
                Type t = typeof(T);
                Type u = Nullable.GetUnderlyingType(t);

                if ((value.IsNullOrEmpty() && u != null))
                    return default(T);
                else
                    return (T)Convert.ChangeType(value, u ?? t);
            }
            catch
            {
                throw new InvalidOperationException(msg);
            }
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool ToBoolean(this string value)
        {
            return !string.IsNullOrEmpty(value) && value == "S";
        }

        public static bool IsUpper(this string value)
        {
            // Consider string to be uppercase if it has no lowercase letters.
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsLower(value[i]))
                    return false;
            }
            return true;
        }
    }
}
