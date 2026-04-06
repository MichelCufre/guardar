using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class LocacionGlobalValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public LocacionGlobalValidationRule(IUnitOfWork uow, string value)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var locacion = long.Parse(this._value);

            var errors = new List<IValidationError>();

            if (!IsValidGLN(locacion))
                errors.Add(new ValidationError("General_Sec0_Error_LocationNumerberIncorrecto"));

            return errors;
        }


        /// <summary>
        /// Comprueba si un codigo GLN (Global Location Number) es válido. Esto se cumple si el último digito es correcto para el resto de los numeros del código.
        /// </summary>
        /// <param name="value">Valor a comprobar</param>
        /// <returns>True si el digito verificador es correcto, false de lo contrario</returns>
        public static bool IsValidGLN(long value)
        {
            string strValue = Convert.ToString(value);
            int checkDigit = Convert.ToInt32(strValue.Last().ToString());
            int totalValue = 0;

            strValue = strValue.Substring(0, strValue.Length - 1);

            bool even = false;

            //Generar el total sumando los digitos. Los digitos de ubicación par se multiplican por 3
            foreach (char c in strValue)
            {
                int charVal = Convert.ToInt32(c.ToString());

                charVal = (even ? 3 * charVal : charVal);

                totalValue = totalValue + charVal;

                even = !even;
            }

            //Restar mayor multiplo de 10 mas cercano al total

            int diff = ((int)(Math.Ceiling(totalValue / 10d) * 10)) - totalValue;

            return diff == checkDigit;
        }


    }
}