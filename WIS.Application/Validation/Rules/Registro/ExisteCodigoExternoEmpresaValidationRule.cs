using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteCodigoExternoEmpresaValidationRule : IValidationRule 
    {
        protected readonly string _codigoExterno;
        protected readonly int _codigoEmpresa;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoExternoEmpresaValidationRule(IUnitOfWork uow, string codigoExterno, int codigoEmpresa)
        {
            this._codigoEmpresa = codigoEmpresa;
            this._codigoExterno = codigoExterno;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.RecepcionTipoRepository.AnyEmpresaRecepcionTipo(_codigoEmpresa, _codigoExterno))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoExternoEmpresaExistente"));

            return errors;
        }

    }
}
