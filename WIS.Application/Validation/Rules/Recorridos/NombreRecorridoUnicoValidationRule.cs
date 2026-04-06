using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Recorridos;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recorridos
{
    public class NombreRecorridoUnicoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int? _nuRecorrido;
        protected readonly IUnitOfWork _uow;

        public NombreRecorridoUnicoValidationRule(IUnitOfWork uow, string value, int? nuRecorrido = null)
        {
            this._uow = uow;
            this._value = value;
            this._nuRecorrido = nuRecorrido;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.RecorridoRepository.ExisteNombreRecorrido(_value, _nuRecorrido))
                errors.Add(new ValidationError("REG700_msg_Error_NombreRecorridoExistente"));
            else if (_value.StartsWith(RecorridosConstants.DEFAULT, StringComparison.InvariantCultureIgnoreCase))
                errors.Add(new ValidationError("REG700_msg_Error_NombreRecorridoInvalido", new List<string>() { RecorridosConstants.DEFAULT }));

            return errors;
        }
    }
}
