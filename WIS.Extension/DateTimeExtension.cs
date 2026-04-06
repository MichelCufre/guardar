using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Convierte la fecha nuleable seleccionada en string
        /// </summary>
        /// <param name="value">Fecha de tipo DateTime nuleable</param>
        /// <param name="format">Formato de salida como string</param>
        /// <returns>Fecha formateada como string si la fecha no es nula, un string vacío de lo contrario</returns>
        public static string ToString(this DateTime? value, string format)
        {
            if (value == null)
                return string.Empty;

            DateTime realValue = value ?? DateTime.Now; //Nunca entra en else

            return realValue.ToString(format);
        }
        /// <summary>
        /// Convierte fecha a formato ISO 8601
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToIsoString(this DateTime value)
        {
            return value.ToString("O");
        }
        /// <summary>
        /// Convierte fecha a formato ISO 8601
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToIsoString(this DateTime? value)
        {
            if (value == null)
                return string.Empty;

            DateTime realValue = value ?? DateTime.Now; //Nunca entra en else

            return realValue.ToString("O");
        }

        public static bool TryParseFromIso(this string value, out DateTime? parsedValue)
        {
            parsedValue = null;

            try
            {
                parsedValue = DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);

                return true;
            }
            catch (Exception)
            {
                //TODO: Logging
                return false;
            }
        }

        /// <summary>
        /// Intenta convertir fecha string en formato ISO a Datetime 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parsedValue"></param>
        /// <returns></returns>

        public static bool TryParseFromIso(this string value, out DateTime parsedValue)
        {
            parsedValue = new DateTime();
            try
            {
                parsedValue = DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
                return true;
            }
            catch (Exception)
            {
                //TODO: Logging
                return false;
            }
        }

        public static DateTime? ParseFromIso(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        public static string ToFilterable(this DateTime value)
        {
            return value.ToString("dd/MM/yyyy HH:mm:ss");
        }
        public static string ToFilterableNullable(this DateTime? value)
        {
            if (value == null)
                return string.Empty;

            return value.ToString("dd/MM/yyyy HH:mm:ss");
        }


        public static DateTime? FromString_DDMMYYYY(string date, IFormatProvider culture)
        {
            date = date.Replace("/", "");
            date = date.Replace("-", "");
            string dayStr, monthStr, yearStr;
            int day, month, year;

            if (date.Length < 8)
                return null;

            dayStr = date.Substring(0, 2);
            monthStr = date.Substring(2, 2);
            yearStr = date.Substring(4, 4);

            if (!int.TryParse(dayStr, out day))
                return null;

            if (!int.TryParse(monthStr, out month))
                return null;

            if (!int.TryParse(yearStr, out year))
                return null;

            return DateTime.ParseExact(date, "ddMMyyyy", culture);
        }

        public static bool IsValid_DDMMYYYY(string date, IFormatProvider culture)
        {
            date = date.Replace("/", "");
            date = date.Replace("-", "");
            string dayStr, monthStr, yearStr;
            int day, month, year;

            if (date.Length < 8)
                return false;

            dayStr = date.Substring(0, 2);
            monthStr = date.Substring(2, 2);
            yearStr = date.Substring(4, 4);

            if (!int.TryParse(dayStr, out day))
                return false;

            if (!int.TryParse(monthStr, out month))
                return false;

            if (!int.TryParse(yearStr, out year))
                return false;

            try
            {
                DateTime.ParseExact(date, "ddMMyyyy", culture);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool IsValid_ImportExcel(string date, string[] formats, DateTimeStyles style, IFormatProvider culture)
        {
            try
            {
                if (!DateTime.TryParseExact(date, formats, culture, style, out DateTime startDate))
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
