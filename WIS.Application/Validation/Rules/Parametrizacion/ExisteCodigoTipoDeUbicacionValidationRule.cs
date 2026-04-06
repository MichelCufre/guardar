using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Parametrizacion
{
    public class ExisteCodigoTipoDeUbicacionValidationRule : IValidationRule
    {
        protected readonly string _codigoTipoUbicacion;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoTipoDeUbicacionValidationRule(string codigoTipoUbicacion, IUnitOfWork uow)
        {
            this._codigoTipoUbicacion = codigoTipoUbicacion;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if(!string.IsNullOrEmpty(_codigoTipoUbicacion))
                if (_uow.UbicacionTipoRepository.AnyUbicacionTipo(short.Parse(this._codigoTipoUbicacion)))
                    errors.Add(new ValidationError("General_Sec0_Error_TipoCodUbicacionExiste"));

            return errors;
        }
    }
}
