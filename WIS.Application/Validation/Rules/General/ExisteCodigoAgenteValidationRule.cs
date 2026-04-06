using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteCodigoAgenteValidationRule : IValidationRule
    {
        protected readonly string _agente;
        protected readonly string _tipo;
        protected readonly string _empresa;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoAgenteValidationRule(IUnitOfWork uow, string agente, string tipo, string empresa)
        {
            this._agente = agente;
            this._tipo = tipo;
            this._empresa = empresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._tipo) && !string.IsNullOrEmpty(this._empresa))
            {
                if (!_uow.AgenteRepository.AnyAgente(this._agente, this._tipo, int.Parse(this._empresa)))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_AgenteTipoEmpresaNoExiste", new List<string> { _agente, _tipo, _empresa }));
                }
            }
            else if (string.IsNullOrEmpty(this._tipo) && !string.IsNullOrEmpty(this._empresa))
            {
                if (!_uow.AgenteRepository.AnyAgente(this._agente, int.Parse(this._empresa)))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_AgenteEmpresaNoExiste", new List<string> { _agente, _empresa }));
                }
            }
            else if (!string.IsNullOrEmpty(this._tipo) && string.IsNullOrEmpty(this._empresa))
            {
                if (!_uow.AgenteRepository.AnyAgente(this._agente, this._tipo))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_AgenteTipoNoExiste", new List<string> { _agente, _tipo }));
                }
            }
            else
            {
                if (!_uow.AgenteRepository.AnyAgente(this._agente))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_AgenteNoExiste", new List<string> { _agente }));
                }
            }

            return errors;
        }
    }
}
