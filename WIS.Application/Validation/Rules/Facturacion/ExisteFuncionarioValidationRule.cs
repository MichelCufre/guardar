using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    class ExisteFuncionarioValidationRule : IValidationRule
    {
        protected readonly int _cdFuncionario;
        protected readonly IUnitOfWork _uow;
        

        public ExisteFuncionarioValidationRule(IUnitOfWork uow, int cdFuncionario)
        {
            this._uow = uow;
            this._cdFuncionario = cdFuncionario;
            
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.FuncionarioRepository.ExisteFuncionario(this._cdFuncionario))
                errors.Add(new ValidationError("General_Sec0_Error_FuncionarioNoExiste"));

            return errors;
        }
    }
}
