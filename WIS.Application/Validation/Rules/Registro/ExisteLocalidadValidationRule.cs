using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteLocalidadValidationRule : IValidationRule
    {
        protected readonly long _value;
        protected readonly string _valuePais;
        protected readonly string _valueSubdivision;
        protected readonly IUnitOfWork _uow;

        public ExisteLocalidadValidationRule(IUnitOfWork uow, long codigoLocalidad, string codigoPais, string codigoSubdivision)
        {
            this._value = codigoLocalidad;
            this._valuePais = codigoPais;
            this._valueSubdivision = codigoSubdivision;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._valuePais) && string.IsNullOrEmpty(this._valueSubdivision))
            {
                if (!_uow.PaisSubdivisionLocalidadRepository.AnyLocalidad(_value))
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoLocalidadNoExistente"));
            }
            else if (!string.IsNullOrEmpty(this._valueSubdivision))
            {
                if (!_uow.PaisSubdivisionLocalidadRepository.AnyLocalidad(this._value, this._valueSubdivision))
                {
                    var subdivision = _uow.PaisSubdivisionRepository.GetPaisSubdivision(this._valueSubdivision);
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoLocalidadEnSubdivisionNoExistente", new List<string>() { subdivision.Nombre }));
                }
            }
            else if (!string.IsNullOrEmpty(this._valuePais))
            {
                if (!_uow.PaisSubdivisionLocalidadRepository.AnyLocalidadEnPais(this._value, this._valuePais))
                {
                    var pais = _uow.PaisRepository.GetPais(this._valuePais);
                    errors.Add(new ValidationError("General_Sec0_Error_CodigoLocalidadEnPaisNoExistente", new List<string>() { pais.Nombre }));
                }
            }

            return errors;
        }
    }
}