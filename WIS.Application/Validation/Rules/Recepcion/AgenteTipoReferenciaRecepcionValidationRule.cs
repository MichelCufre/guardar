using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class AgenteTipoReferenciaRecepcionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _tipoReferencia;
        protected readonly IUnitOfWork _uow;

        public AgenteTipoReferenciaRecepcionValidationRule(IUnitOfWork uow, string codigoInernoAgente, string tipoReferencia)
        {
            this._value = codigoInernoAgente;
            this._tipoReferencia = tipoReferencia;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            string tipoAgente = this._uow.AgenteRepository.GetTipoAgente(this._value);

            if (!this._uow.ReferenciaRecepcionRepository.AnyRecepcionTipo(this._tipoReferencia, tipoAgente))
                errors.Add(new ValidationError("General_Sec0_Error_TipoReferenciaNoValidoParaAgente"));

            return errors;
        }
    }
}