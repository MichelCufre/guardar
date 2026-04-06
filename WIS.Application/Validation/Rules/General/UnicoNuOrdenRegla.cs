using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class UnicoNuOrdenRegla : IValidationRule
    {
        protected readonly string _valueNuOrden;
        protected readonly int? _valueNuRegla;
        protected readonly IUnitOfWork _uow;

        public UnicoNuOrdenRegla(IUnitOfWork uow, string valueNuorden, int? nuRegla)
        {
            this._valueNuOrden = valueNuorden;
            this._valueNuRegla = nuRegla;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (_uow.LiberacionRepository.AnyOrden(short.Parse(_valueNuOrden), _valueNuRegla))
                errors.Add(new ValidationError("General_Sec0_Error_Error63"));
            return errors;
        }
    }
}
