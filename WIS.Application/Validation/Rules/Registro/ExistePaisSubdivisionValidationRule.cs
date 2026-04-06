using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExistePaisSubdivisionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _valuePais;
        protected readonly IUnitOfWork _uow;

        public ExistePaisSubdivisionValidationRule(IUnitOfWork uow, string codigoPaisSubdivision, string codigoPais)
        {
            this._value = codigoPaisSubdivision;
            this._valuePais = codigoPais;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._valuePais))
            {
                if (!_uow.PaisSubdivisionRepository.AnyPaisSubdivision(this._value))
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoPaisSubdivisionNoExistente"));
            }
            else
            {
                if (!_uow.PaisSubdivisionRepository.AnyPaisSubdivision(this._value, this._valuePais))
                {
                    var pais = _uow.PaisRepository.GetPais(this._valuePais);
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoPaisSubdivisionEnPaisNoExistente", new List<string>() { pais.Nombre }));
                }

            }



            return errors;
        }
    }
}