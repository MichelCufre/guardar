using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;
namespace WIS.Application.Validation.Rules.General
{
    public class ExisteIdUsuarioValidationRule : IValidationRule
    {
        protected readonly int _idUsuario;
        protected readonly IUnitOfWork _uow;

        public ExisteIdUsuarioValidationRule(IUnitOfWork uow, string idUsuario)
        {
            this._idUsuario = int.Parse(idUsuario);
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.SecurityRepository.AnyUsuario(this._idUsuario))
                errors.Add(new ValidationError("General_Sec0_Error_UsuarioExistente"));

            return errors;
        }
    }
}
