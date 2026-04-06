using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class NombreAtributoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int? _idAtributo;
        protected readonly IUnitOfWork _uow;

        public NombreAtributoValidationRule(IUnitOfWork uow, string value, int? idAtributo = null)
        {
            _uow = uow;
            _value = value;
            _idAtributo = idAtributo;
        }


        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (_uow.AtributoRepository.AnyAtributoNombre(_value, _idAtributo))
                errors.Add(new ValidationError("PAR401_Sec0_Error_NombreAtributoExistente"));

            return errors;
        }
    }
}
