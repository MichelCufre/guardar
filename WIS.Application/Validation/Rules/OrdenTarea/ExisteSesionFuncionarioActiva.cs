using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    public class ExisteSesionFuncionarioActiva : IValidationRule
    {
        protected readonly int _userId;
        protected readonly IUnitOfWork _uow;

        public ExisteSesionFuncionarioActiva(IUnitOfWork uow, int userId)
        {
            this._userId = userId;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.TareaRepository.AnySesionActivaFuncionario(this._userId))
                errors.Add(new ValidationError("General_ORT090_Error_UsuarioConSesionActiva"));
            else if (_uow.TareaRepository.AnySesionActivaFuncionarioAuxiliar(this._userId, out string userSesionActiva))
                errors.Add(new ValidationError("General_ORT090_Error_UsuarioAuxConSesionActiva", new List<string>() { userSesionActiva }));
            return errors;
        }
    }
}
