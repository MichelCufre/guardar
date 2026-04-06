using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class UsuarioAsociadoAEmpresaValidationRule : IValidationRule
    {
        protected readonly int _idUsuario;
        protected readonly int _idEmpresa;
        protected readonly IUnitOfWork _uow;

        public UsuarioAsociadoAEmpresaValidationRule(IUnitOfWork uow, string userId, string idEmpresa)
        {
            this._idUsuario = int.Parse(userId);
            this._idEmpresa = int.Parse(idEmpresa);
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.SecurityRepository.AnyEmpresaUsuario(_idUsuario, _idEmpresa))
                errors.Add(new ValidationError("General_Sec0_Error_UsuarioNoManejaEmpresa"));

            return errors;
        }
    }
}
