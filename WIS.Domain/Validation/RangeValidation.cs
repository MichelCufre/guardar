using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class RangeValidation : RangeAttribute
    {
        public RangeValidation(int maxLength, int precision = 0, bool distintoCero = true, bool noNegativo = true) : base(typeof(decimal), GetMinValue(maxLength, precision, distintoCero, noNegativo).ToString(CultureInfo.InvariantCulture), GetMaxValue(maxLength, precision).ToString(CultureInfo.InvariantCulture))
        {
            ParseLimitsInInvariantCulture = true;
            ConvertValueInInvariantCulture = true;
        }

        public static decimal GetMaxValue(int maxLength, int precision)
        {
            var maxValue = (decimal)Math.Pow(10, maxLength) - 1m;

            if (precision > 0 && maxLength > precision)
            {
                maxValue = Math.Round(((decimal)Math.Pow(10, (maxLength - precision)) - 1m) + ((decimal)Math.Pow(10, precision) - 1m) / (decimal)Math.Pow(10, precision), precision);
            }

            return maxValue;
        }

        public static decimal GetMinValue(int maxLength, int precision, bool distintoCero, bool noNegativo)
        {
            var minValue = -(decimal)Math.Pow(10, maxLength) + 1m;

            if (noNegativo)
            {
                if (distintoCero)
                {
                    if (precision > 0 && maxLength > precision)
                    {
                        minValue = Math.Round(1m - ((decimal)Math.Pow(10, precision) - 1m) / (decimal)Math.Pow(10, precision), precision);
                    }
                    else
                    {
                        minValue = 1m;
                    }
                }
                else
                {
                    minValue = 0m;
                }
            }
            else
            {
                if (precision > 0 && maxLength > precision)
                {
                    minValue = -(Math.Round(((decimal)Math.Pow(10, (maxLength - precision)) - 1m) + ((decimal)Math.Pow(10, precision) - 1m) / (decimal)Math.Pow(10, precision), precision));
                }
            }

            return minValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validationService = validationContext.GetService(typeof(IValidationService)) as IValidationService;
            var result = base.IsValid(value as Nullable<Decimal>, validationContext);

            if (result != null)
            {
                var minimum = Convert.ToString(Minimum, CultureInfo.InvariantCulture);
                var maximum = Convert.ToString(Maximum, CultureInfo.InvariantCulture);

                var error = new Error("WMSAPI_msg_Error_RangoValidation", validationContext.MemberName, minimum, maximum);

                ErrorMessage = validationService.Translate(error);
            }

            return result;
        }
    }
}

