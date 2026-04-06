using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteTransportadoraValidationRule : IValidationRule
    {
        protected readonly string _valueTransportista;
        protected readonly IUnitOfWork _uow;

        public ExisteTransportadoraValidationRule(IUnitOfWork uow, string valueTransportista)
        {
            this._valueTransportista = valueTransportista;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var cdTransportista = int.Parse(this._valueTransportista);

            var errors = new List<IValidationError>();

            if (!this._uow.TransportistaRepository.AnyTransportista(cdTransportista))
                errors.Add(new ValidationError("General_Sec0_Error_IdTransportistaNoExistente"));

            return errors;
        }
    }
}
