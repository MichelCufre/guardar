using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteResultadoManualValidationRule : IValidationRule
    {
        protected readonly int _cdEmpresa;
        protected readonly string _cdFacturacion;
        protected readonly int _numeroEjecucion;
        protected readonly string _componente;
        protected readonly IUnitOfWork _uow;

        public ExisteResultadoManualValidationRule(IUnitOfWork uow, int valueEmpresa, string valueFacturacion, int valueEjecucion, string valueComponente)
        {
            this._cdEmpresa = valueEmpresa;
            this._cdFacturacion = valueFacturacion;
            this._numeroEjecucion = valueEjecucion;
            this._componente = valueComponente;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.FacturacionRepository.AnyFacturacionResultado(this._cdEmpresa, this._cdFacturacion, this._numeroEjecucion, this._componente))
                errors.Add(new ValidationError("General_Sec0_Error_ResultadoManualExiste"));

            return errors;
        }
    }
}
