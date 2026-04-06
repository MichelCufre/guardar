using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteClienteValidationRule : IValidationRule
    {
        protected readonly string _valueCliente;
        protected readonly string _valueEmpresa;
        protected readonly IUnitOfWork _uow;

        public ExisteClienteValidationRule(IUnitOfWork uow, string valueCliente, string valueEmpresa)
        {
            this._valueCliente = valueCliente;
            this._valueEmpresa = valueEmpresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.AgenteRepository.AnyCliente(this._valueCliente, int.Parse(this._valueEmpresa)))
            {
                if (_uow.AgenteRepository.AnyCliente(this._valueCliente))
                    errors.Add(new ValidationError("General_Sec0_Error_ClienteNoPerteneceEmpresa", new List<string> { _valueCliente, _valueEmpresa }));
                else
                    errors.Add(new ValidationError("General_Sec0_Error_ClienteNoExiste", new List<string> { _valueCliente }));
            }

            return errors;
        }
    }
}
