using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
	public class EspacioProduccionExisteValidationRule : IValidationRule
	{
		protected readonly IUnitOfWork _uow;
		protected readonly string _value;

		public EspacioProduccionExisteValidationRule(IUnitOfWork uow, string value)
		{
			_uow = uow;
			_value = value;
		}

		public virtual List<IValidationError> Validate()
		{
			List<IValidationError> errors = new List<IValidationError>();

			var espacio = _uow.EspacioProduccionRepository.GetEspacioProduccion(_value);

			if (espacio == null) errors.Add(new ValidationError("PRD500_Sec0_Error_EspacioPrdNoExiste"));

			return errors;
		}
	}
}
