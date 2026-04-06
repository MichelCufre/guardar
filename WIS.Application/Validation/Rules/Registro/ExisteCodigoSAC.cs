using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteCodigoSAC : IValidationRule
    {
        protected readonly string _idCodigoSAC;
        protected readonly IUnitOfWork _uow;


        public ExisteCodigoSAC(IUnitOfWork uow, string idCodigoSAC)
        {
            this._uow = uow;
            this._idCodigoSAC = idCodigoSAC;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.NcmRepository.ExisteNCM(this._idCodigoSAC))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoSACExiste"));

            return errors;
        }

    }
}
