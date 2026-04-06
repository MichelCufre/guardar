using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class TransferenciaEmpresaIngresoValidationRule : IValidationRule
    {
        protected readonly string _empresaIngreso;
        protected readonly string _empresaEgreso;

        public TransferenciaEmpresaIngresoValidationRule(string empresaIngreso, string empresaEgreso)
        {
            this._empresaIngreso = empresaIngreso;
            this._empresaEgreso = empresaEgreso;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._empresaIngreso) && !string.IsNullOrEmpty(this._empresaEgreso))
            {
                if (this._empresaIngreso == this._empresaEgreso)
                    errors.Add(new ValidationError("General_Sec0_Error_TransferenciaEmpresaIngreso"));
            }

            return errors;
        }
    }
}
