using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Filtering;
using WIS.Filtering.Expressions;
using WIS.Persistence.Extensions;
using WIS.Security;

namespace WIS.BackendService.Services
{
    public class ExpressionLiteralConverter : IExpressionLiteralConverter
    {
        protected readonly IIdentityService _identity;

        public ExpressionLiteralConverter(IIdentityService identity)
        {
            _identity = identity;
        }

        public Expression ConvertType(ConstantExpression expression, Type type)
        {
            string value = (string)expression.Value;

            if (type == typeof(string))
                return expression;

            dynamic convertedValue = this.ConvertDataType(value, type);

            if (Nullable.GetUnderlyingType(type) == null && convertedValue == null) //Tipo no es nulleable, pero el valor es nulleable
                throw new InvalidOperationException("Field is not nullable");

            return Expression.Constant(convertedValue, type);
        }

        public Expression ConvertTypeGroup(ConstantExpression expression, Type type)
        {
            List<string> values = (List<string>)expression.Value;

            Type listType = typeof(List<>).MakeGenericType(new[] { type });

            IList list = (IList)Activator.CreateInstance(listType);

            foreach (var value in values)
            {
                list.Add(this.ConvertDataType(value, type));
            }

            return Expression.Constant(list);
        }

        public Expression ConvertNumberType(MemberExpression expression, Type type)
        {
            if (type == typeof(string))
            {
                if (expression.Type == typeof(int))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("IntToChar"), expression);

                if (expression.Type == typeof(int?))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("NullableIntToChar"), expression);

                if (expression.Type == typeof(short))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("ShortToChar"), expression);

                if (expression.Type == typeof(short?))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("NullableShortToChar"), expression);

                if (expression.Type == typeof(decimal))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("DecimalToChar"), expression);

                if (expression.Type == typeof(decimal?))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("NullableDecimalToChar"), expression);

                if (expression.Type == typeof(double))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("DoubleToChar"), expression);

                if (expression.Type == typeof(double?))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("NullableDoubleToChar"), expression);

                if (expression.Type == typeof(long))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("LongToChar"), expression);

                if (expression.Type == typeof(long?))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("NullableLongToChar"), expression);
            }

            return expression;
        }

        public Expression ConvertDateType(MemberExpression expression, Type type)
        {
            if (type == typeof(string))
            {
                if (expression.Type == typeof(DateTime) || expression.Type == typeof(DateTime?))
                    return Expression.Call(typeof(CustomDbFunctions).GetMethod("NullableDateToChar"), expression);
            }

            return expression;
        }

        public Expression ConvertDatePartialKeyword(ConstantExpression expression)
        {
            var value = (string)expression.Value;

            if (value.StartsWith($"{FilterTypes.Day}(") && value.EndsWith(")"))
            {
                var offsetText = value.Substring(4, value.Length - 5);
                if (int.TryParse(offsetText, out int offset))
                {
                    return Expression.Constant(DateTime.Today.AddDays(offset).ToString(CDateFormats.DATE_ONLY));
                }
            }

            switch (value)
            {
                case FilterTypes.Today: return Expression.Constant(DateTime.Today.ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.Tomorrow: return Expression.Constant(DateTime.Today.AddDays(1).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.Yesterday: return Expression.Constant(DateTime.Today.AddDays(-1).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.DayAfterTomorrow: return Expression.Constant(DateTime.Today.AddDays(2).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.DayBeforeYesterday: return Expression.Constant(DateTime.Today.AddDays(-2).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.StartOfWeek: return Expression.Constant(DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.EndOfWeek: return Expression.Constant(DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.StartOfMonth: return Expression.Constant(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.MidMonth: return Expression.Constant(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.EndOfMonth: return Expression.Constant(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.StartOfYear: return Expression.Constant(new DateTime(DateTime.Today.Year, 1, 1).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.MidYear: return Expression.Constant(new DateTime(DateTime.Today.Year, 7, 1).ToString(CDateFormats.DATE_ONLY));
                case FilterTypes.EndOfYear: return Expression.Constant(new DateTime(DateTime.Today.Year, 12, 31).ToString(CDateFormats.DATE_ONLY));
                default: return expression;
            }
        }

        public dynamic ConvertDataType(string value, Type type)
        {
            if (value == null)
                return null;

            if (type == typeof(string))
                return value;

            if (type == typeof(int))
                return int.Parse(value);

            if (type == typeof(int?))
                return int.Parse(value);

            if (type == typeof(decimal))
                return decimal.Parse(value, _identity.GetFormatProvider());

            if (type == typeof(decimal?))
                return decimal.Parse(value, _identity.GetFormatProvider());

            if (type == typeof(double))
                return double.Parse(value, _identity.GetFormatProvider());

            if (type == typeof(double?))
                return double.Parse(value, _identity.GetFormatProvider());

            if (type == typeof(short))
                return short.Parse(value);

            if (type == typeof(short?))
                return short.Parse(value);

            if (type == typeof(long))
                return long.Parse(value);

            if (type == typeof(long?))
                return long.Parse(value);

            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return this.ConvertDate(value);

            return value;
        }

        public DateTime ConvertDate(string value)
        {
            if (value.StartsWith($"{FilterTypes.Day}(") && value.EndsWith(")"))
            {
                var offsetText = value.Substring(4, value.Length - 5);
                if (int.TryParse(offsetText, out int offset))
                {
                    return DateTime.Today.AddDays(offset);
                }
            }

            switch (value)
            {
                case FilterTypes.Today: return DateTime.Today;
                case FilterTypes.Tomorrow: return DateTime.Today.AddDays(1);
                case FilterTypes.Yesterday: return DateTime.Today.AddDays(-1);
                case FilterTypes.DayAfterTomorrow: return DateTime.Today.AddDays(2);
                case FilterTypes.DayBeforeYesterday: return DateTime.Today.AddDays(-2);
                case FilterTypes.StartOfWeek: return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
                case FilterTypes.EndOfWeek: return DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek);
                case FilterTypes.StartOfMonth: return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                case FilterTypes.MidMonth: return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15);
                case FilterTypes.EndOfMonth: return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
                case FilterTypes.StartOfYear: return new DateTime(DateTime.Today.Year, 1, 1);
                case FilterTypes.MidYear: return new DateTime(DateTime.Today.Year, 7, 1);
                case FilterTypes.EndOfYear: return new DateTime(DateTime.Today.Year, 12, 31);
            }

            var ci = new CultureInfo("es-UY");
            var formatString = CDateFormats.DATETIME_24H;

            if (value.ToString().Length == 10)
            {
                formatString = CDateFormats.DATE_ONLY;
            }
            else if (value.ToString().Length > 10)
            {
                value = Regex.Replace(value, " +", " "); //Quitar espacios duplicados

                string[] parts = value.Split(' ');

                if (parts.Length > 1)
                {
                    string[] timeParts = parts[1].Split(':');

                    var result = new List<string>();

                    foreach (var part in timeParts)
                    {
                        if (part.Length < 2)
                            result.Add("0" + part);
                        else
                            result.Add(part);
                    }

                    while (result.Count < 3)
                    {
                        result.Add("00");
                    }

                    value = $"{parts[0]} {string.Join(":", result)}";
                }
            }

            return DateTime.ParseExact(value, formatString, ci);
        }
    }
}
