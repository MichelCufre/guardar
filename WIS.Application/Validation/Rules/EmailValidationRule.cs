using System.Collections.Generic;
using System.Text.RegularExpressions;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class EmailValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int? _userId;
        protected readonly int? _empresa;
        protected readonly bool _allowNull;
        protected readonly bool _validarMailEnUso;
        protected readonly bool _validacionUsuario;
        protected readonly IUnitOfWork _uow;

        public EmailValidationRule(IUnitOfWork uow, string value, bool validacionUsuario, bool allowNull = false, bool validarMailEnUso = true, int? userId = null, int? empresa = null)
        {
            this._uow = uow;
            this._value = value;
            this._userId = userId;
            this._empresa = empresa;
            this._allowNull = allowNull;
            this._validarMailEnUso = validarMailEnUso;
            this._validacionUsuario = validacionUsuario;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_allowNull && string.IsNullOrEmpty(_value))
                return errors;

            if (!Regex.IsMatch(_value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                errors.Add(new ValidationError("General_Sec0_Error_Error14"));

            if (_validarMailEnUso)
            {
                if (_validacionUsuario && _uow.SecurityRepository.AnyUserMail(_value, _userId))
                    errors.Add(new ValidationError("SEC030_Sec0_Error_MailExistente"));
                else if (!_validacionUsuario && _uow.DestinatarioRepository.AnyMailEmpresa(_value, _empresa))
                    errors.Add(new ValidationError("EVT020_Sec0_Error_MailExistente"));
            }

            return errors;
        }
    }
}
