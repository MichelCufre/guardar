using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ModalidadIngresoLoteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _tpManejoIdent;
        protected readonly IUnitOfWork _uow;

        public ModalidadIngresoLoteValidationRule(IUnitOfWork uow, string value, string tpManejoIdent)
        {
            this._value = value;
            this._tpManejoIdent = tpManejoIdent;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_value) && !string.IsNullOrEmpty(_tpManejoIdent))
            {
                if (_tpManejoIdent != ManejoIdentificadorDb.Lote && _value != ModalidadIngresoLoteDb.Normal)
                    errors.Add(new ValidationError("REG009_Sec0_Error_ModLoteIncompatibleConTpIdent", new List<string> { this._tpManejoIdent, this._value }));
            }

            return errors;
        }
    }
}
