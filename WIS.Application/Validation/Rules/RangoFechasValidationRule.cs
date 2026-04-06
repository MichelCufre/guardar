using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class RangoFechasValidationRule : IValidationRule
    {
        protected readonly string _valueDateStringDesde;
        protected readonly string _valueDateStringHasta;

        public RangoFechasValidationRule(string valueDateStringDesde,string valueDateStringHasta)
        {
            this._valueDateStringDesde = valueDateStringDesde;
            this._valueDateStringHasta = valueDateStringHasta;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            DateTime? dateDesde;
            DateTime? dateHasta;
            if (this._valueDateStringDesde.TryParseFromIso(out dateDesde) && this._valueDateStringHasta.TryParseFromIso(out dateHasta))
            {
                if(dateDesde > dateHasta)
                    errors.Add(new ValidationError("General_Sec0_Error_Error66"));
            }          

            return errors;
        }
    }
}
