using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdEmpresaNoExistenteGeneralValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdEmpresaNoExistenteGeneralValidationRule(IUnitOfWork uow, string idEmpresa)
        {
            this._value = idEmpresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.EmpresaRepository.AnyEmpresaGeneral(int.Parse(_value)))
                errors.Add(new ValidationError("General_Sec0_Error_IdEmpresaExistente"));

            return errors;
        }
    }
}
