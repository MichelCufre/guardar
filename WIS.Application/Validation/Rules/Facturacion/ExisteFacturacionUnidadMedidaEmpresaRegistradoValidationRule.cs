using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteFacturacionUnidadMedidaEmpresaRegistradoValidationRule : IValidationRule
    {
        protected readonly string _unidadMedida;
        protected readonly int _cdEmpresa;
        protected readonly IUnitOfWork _uow;

        public ExisteFacturacionUnidadMedidaEmpresaRegistradoValidationRule(IUnitOfWork uow, string unidadMedida, int cdEmpresa)
        {
            this._unidadMedida = unidadMedida;
            this._cdEmpresa = cdEmpresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.FacturacionRepository.AnyFacturacionUnidadMedidaEmpresa(this._unidadMedida, this._cdEmpresa))
                errors.Add(new ValidationError("General_Sec0_Error_FacturacionUnidadMedidaEmpresaExiste"));

            return errors;
        }
    }
}
