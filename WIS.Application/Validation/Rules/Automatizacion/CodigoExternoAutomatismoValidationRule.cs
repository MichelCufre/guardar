using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Automatizacion
{
    public class CodigoExternoAutomatismoValidationRule : IValidationRule
    {
        protected IUnitOfWork _uow;
        protected string _value;
        protected int? _nroAutomatimo;

        public CodigoExternoAutomatismoValidationRule(IUnitOfWork uow, string value, int? nroAutomatimo)
        {
            _uow = uow;
            _value = value;
            _nroAutomatimo = nroAutomatimo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_value))
            {
                var isUpdate = _nroAutomatimo.HasValue;
                var automatismo = _uow.AutomatismoRepository.GetAutomatismoByCodigo(_value);

                if (automatismo != null && (!isUpdate || (isUpdate && automatismo.Numero != _nroAutomatimo.Value)))
                    errors.Add(new ValidationError("AUT100Modal_Sec0_Error_CodigoExternoExistente", new List<string>() { _value }));
            }
            return errors;
        }
    }
}
