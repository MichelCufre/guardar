using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteCodigoProceso : IValidationRule
    {
        protected readonly string _idCodigo;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoProceso(IUnitOfWork uow, string valueCodigo)
        {
            this._idCodigo = valueCodigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.FacturacionRepository.AnyCodigoProceso(this._idCodigo))
                errors.Add(new ValidationError("General_Sec0_Error_ProcesoYaExiste"));

            return errors;
        }
    }
}
