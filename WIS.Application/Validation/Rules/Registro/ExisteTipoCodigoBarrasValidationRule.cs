using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteTipoCodigoBarrasValidationRule : IValidationRule
    {
        protected readonly string _idTpBarra;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoCodigoBarrasValidationRule(IUnitOfWork uow, string idTpBarra)
        {
            this._idTpBarra = idTpBarra;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();


            if (string.IsNullOrEmpty(_idTpBarra))
            {
                errors.Add(new ValidationError("General_Sec0_Error_TipoCodigoBarraFaltante"));
            }
            else
            {
                if (!_uow.ProductoCodigoBarraRepository.ExisteTipoCodigoBarras(int.Parse(_idTpBarra)))
                    errors.Add(new ValidationError("General_Sec0_Error_TipoCodigoNoBarraExistente"));
            }

            return errors;
        }
    }
}