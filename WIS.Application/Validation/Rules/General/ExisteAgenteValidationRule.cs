using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteAgenteValidationRule : IValidationRule
    {
        protected readonly string _agente;
        protected readonly string _tipo;
        protected readonly int _empresa;
        protected readonly IUnitOfWork _uow;

        public ExisteAgenteValidationRule(IUnitOfWork uow, string agente, string tipo, int empresa)
        {
            this._agente = agente;
            this._tipo = tipo;
            this._empresa = empresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.AgenteRepository.AnyAgente(this._agente, this._tipo, this._empresa))
            {
                if (_uow.AgenteRepository.AnyAgente(this._agente, this._tipo))
                    errors.Add(new ValidationError("General_Sec0_Error_ClienteNoPerteneceEmpresa", new List<string> { _agente, _empresa.ToString() }));
                else
                    errors.Add(new ValidationError("General_Sec0_Error_ClienteNoExiste", new List<string> { _agente }));
            }

            return errors;
        }
    }
}
