using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CodigoAgenteRule : IValidationRule
    {
        protected readonly string _valueEmpresa;
        protected readonly string _valueTipoAgente;
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public CodigoAgenteRule(IUnitOfWork uow, string valueEmpresa, string valueTipoAgente, string value)
        {
            this._valueTipoAgente = valueTipoAgente;
            this._valueEmpresa = valueEmpresa;
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!int.TryParse(this._valueEmpresa, out int codigoEmpresa))
                return errors;

            if (string.IsNullOrEmpty(_valueTipoAgente))
                return errors;

            if (this._uow.AgenteRepository.AnyAgente(this._value, this._valueTipoAgente, codigoEmpresa))
                errors.Add(new ValidationError("General_Sec0_Error_AgenteExistente"));

            return errors;
        }
    }
}
