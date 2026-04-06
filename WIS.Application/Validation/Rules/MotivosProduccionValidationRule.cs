using WIS.Domain.Produccion.Constants;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
	public class MotivosProduccionValidationRule : IValidationRule
	{
		protected readonly string _value;
		protected readonly IUnitOfWork _uow;

		public MotivosProduccionValidationRule(IUnitOfWork uow, string Value)
		{
			_uow = uow;
			_value = Value;
		}

		public virtual List<IValidationError> Validate()
		{
			List<IValidationError> errors = new List<IValidationError>();

			var motivosPorduccion = _uow.DominioRepository.GetDominios(TipoIngresoProduccion.MOTIVO_PRODUCCION);

			if (!motivosPorduccion.Any(d => d.Id == _value))
				errors.Add(new ValidationError("PRD113_grid1_Error_MotivoProduccionNoExiste"));

			return errors;
		}
	}
}
