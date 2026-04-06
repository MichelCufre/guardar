using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteFacturacionEmpresaProcesoRegistradoValidationRule : IValidationRule
    {
        protected readonly int _cdEmpresa;
        protected readonly string _cdProceso;
        protected readonly IUnitOfWork _uow;

        public ExisteFacturacionEmpresaProcesoRegistradoValidationRule(IUnitOfWork uow, int cdEmpresa, string cdProceso)
        {
            this._uow = uow;
            this._cdEmpresa = cdEmpresa;
            this._cdProceso = cdProceso;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.FacturacionRepository.AnyFacturacionEmpresaProceso(this._cdEmpresa, this._cdProceso))
                errors.Add(new ValidationError("General_Sec0_Error_FacturacionEmpresaProcesoExiste", new List<string> { this._cdProceso, this._cdEmpresa.ToString() }));

            return errors;
        }
    }
}
