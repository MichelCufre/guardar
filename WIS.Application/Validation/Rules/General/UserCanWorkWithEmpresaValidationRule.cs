using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class UserCanWorkWithEmpresaValidationRule : IValidationRule
    {
        protected readonly ISecurityService _security;
        protected readonly string _empresa;

        public UserCanWorkWithEmpresaValidationRule(ISecurityService security, string empresa)
        {
            this._empresa = empresa;
            this._security = security;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._security.IsEmpresaAllowed(int.Parse(this._empresa)))
                errors.Add(new ValidationError("General_Sec0_Error_Er045_UsuarioEmpresaNoAsginada"));

            return errors;
        }
    }
}
