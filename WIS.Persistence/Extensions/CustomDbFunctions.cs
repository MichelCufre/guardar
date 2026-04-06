using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Extensions
{
    public class CustomDbFunctions
    {

        [DbFunction("DATE_TO_CHAR")]
        public static string NullableDateToChar(DateTime? date)
        {
            throw new NotSupportedException();
        }

        [DbFunction("NULLABLE_INT_TO_CHAR")]
        public static string NullableIntToChar(int? number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("INT_TO_CHAR")]
        public static string IntToChar(int number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("SHORT_TO_CHAR")]
        public static string ShortToChar(short number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("NULLABLE_SHORT_TO_CHAR")]
        public static string NullableShortToChar(short? number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("DECIMAL_TO_CHAR")]
        public static string DecimalToChar(decimal number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("NULLABLE_DECIMAL_TO_CHAR")]
        public static string NullableDecimalToChar(decimal? number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("DOUBLE_TO_CHAR")]
        public static string DoubleToChar(double number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("NULLABLE_DOUBLE_TO_CHAR")]
        public static string NullableDoubleToChar(double? number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("LONG_TO_CHAR")]
        public static string LongToChar(long number)
        {
            throw new NotSupportedException();
        }

        [DbFunction("NULLABLE_LONG_TO_CHAR")]
        public static string NullableLongToChar(long? number)
        {
            throw new NotSupportedException();
        }
    }
}
