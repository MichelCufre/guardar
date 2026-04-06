using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Automatizacion
{
    public class TipoAutomatismoValidationRule : IValidationRule
    {
        protected IUnitOfWork _uow;
        protected string _value;
        protected int? _nroAutomatimo;

        public TipoAutomatismoValidationRule(IUnitOfWork uow, string value, int? nroAutomatimo)
        {
            this._uow = uow;
            this._value = value;
            _nroAutomatimo = nroAutomatimo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.DominioRepository.ExisteDetalleDominioValor(AutomatismoDb.TIPO_AUTOMATISMO_DOMAIN, _value))
                errors.Add(new ValidationError("AUT100Modal_Sec0_Error_TipoAutomatismoInexistente"));
            else if (_nroAutomatimo.HasValue)
            {
                var automatismo = _uow.AutomatismoRepository.GetAutomatismoById(_nroAutomatimo.Value);
                if (automatismo.Tipo != AutomatismoTipo.AutoStore && _value == AutomatismoTipo.AutoStore)
                    errors.Add(new ValidationError("AUT100Modal_Sec0_Error_TipoAutomatismoNoCambiable", new List<string>() { automatismo.Tipo, _value }));

            }
            return errors;
        }
    }
}
