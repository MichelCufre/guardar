using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteClienteProveedorEmpresaValidationRule : IValidationRule
    {
        protected readonly string _valueCliente;
        protected readonly string _valueEmpresa;
        protected readonly IUnitOfWork _uow;

        public ExisteClienteProveedorEmpresaValidationRule(IUnitOfWork uow, string valueCliente, string valueEmpresa)
        {
            this._valueCliente = valueCliente;
            this._valueEmpresa = valueEmpresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var cdEmpresa = int.Parse(this._valueEmpresa);

            var errors = new List<IValidationError>();

            //if (!this._context.V_AGENTE.Any(d => d.CD_EMPRESA == cdEmpresa && d.CD_CLIENTE == this._valueCliente))
            //    errors.Add(new ValidationError("General_Sec0_Error_Error17"));

            return errors;
        }
    }
}
