using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class TipoEtiquetaValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public TipoEtiquetaValidationRule(string value, IUnitOfWork uow)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (!string.IsNullOrEmpty(this._value))
            {
                if (this._value.ToUpper() == GeneralDb.WIS)
                    errors.Add(new ValidationError("General_Sec0_Error_Error93", new List<string> { GeneralDb.WIS }));
                else if(_uow.ManejoLpnRepository.ExistePrefijoEtiqueta(this._value.ToUpper()))
                    errors.Add(new ValidationError("General_Sec0_Error_Error94"));
            }

            return errors;
        }
    }
}
