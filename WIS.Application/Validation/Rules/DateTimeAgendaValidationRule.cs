using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DateTimeAgendaValidationRule : IValidationRule
    {
        private string DATE_ONLY = "dd/MM/yyyy";
        protected readonly DateTime? _valueDateString;
        protected readonly string _flControlarVencimiento;
        protected readonly short? _diasValidez;

        public DateTimeAgendaValidationRule(DateTime? valueDateString, string flControlarVencimiento, short? diasValidez)
        {
            this._valueDateString = valueDateString;
            this._flControlarVencimiento = flControlarVencimiento;
            this._diasValidez = diasValidez;
        }

        public virtual List<IValidationError> Validate()
        {
            var fechaActual = DateTime.Today;
            var errors = new List<IValidationError>();
            DateTime date = _valueDateString ?? DateTime.Now;
            if (_flControlarVencimiento == "S")
            {

                if (_valueDateString < fechaActual)
                    errors.Add(new ValidationError("REC410_Sec0_Error_COL19_ErrorConfirmarRecepcion"));

                var fechaLimite = date.AddDays(-1 * (_diasValidez ?? 0));
                if (fechaLimite < fechaActual)
                {
                    errors.Add(new ValidationError("REC410_Sec0_Error_COL20_ErrorConfirmarRecepcion", new List<string> { fechaLimite.ToString(DATE_ONLY) }));
                }
            }
            else if (_flControlarVencimiento == "A")
            {
                if (date < fechaActual)
                    errors.Add(new ValidationError("REC410_Sec0_Error_COL19_ErrorConfirmarRecepcion"));
            }


            return errors;
        }
    }
}
