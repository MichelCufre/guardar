using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class NumeroReferenciaNoExistenteValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;
        protected readonly string _idEmpresa;
        protected readonly string _codigoInternoAgente;
        protected readonly string _tipoReferencia;


        public NumeroReferenciaNoExistenteValidationRule(IUnitOfWork uow, string idEmpresa, string codigoInternoAgente, string tipoReferencia, string numeroReferencia)
        {
            this._uow = uow;
            this._value = numeroReferencia;
            this._idEmpresa = idEmpresa;
            this._codigoInternoAgente = codigoInternoAgente;
            this._tipoReferencia = tipoReferencia;

        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ReferenciaRecepcionRepository.AnyReferencia(int.Parse(_idEmpresa), _codigoInternoAgente, _tipoReferencia, _value))
                errors.Add(new ValidationError("General_Sec0_Error_NumeroReferenciaExistente"));

            return errors;
        }
    }
}