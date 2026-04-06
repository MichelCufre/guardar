using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class IntBetweenValidationRule : IValidationRule
    {
        protected readonly string _number;
        protected readonly int _maximo;
        protected readonly int _minimo;
        public IntBetweenValidationRule(string valor, int minimo, int maximo)
        {
            this._number = valor;
            this._maximo = maximo;
            this._minimo = minimo;
        }
        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (int.TryParse(_number, out int aux))
            {
                if (aux > _maximo)
                    errors.Add(new ValidationError("General_Sec0_Error_ValorMaximo", new List<string> { _maximo.ToString() }));
                if (aux < _minimo)
                    errors.Add(new ValidationError("General_Sec0_Error_ValorMinimo", new List<string> { _minimo.ToString() }));
            }
            else
                errors.Add(new ValidationError("General_Sec0_Error_Error14"));

            return errors;
        }
    }
}
