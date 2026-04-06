using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Produccion.Rules
{
	public class ProductoEsRequeridoValidationRule : IValidationRule
	{
		protected readonly string _value;
		protected readonly string _idIngreso;
		protected readonly IUnitOfWork _uow;

		public ProductoEsRequeridoValidationRule(IUnitOfWork uow, string Value, string idIngreso)
		{
			_uow = uow;
			_value = Value;
			_idIngreso = idIngreso;
		}

		public virtual List<IValidationError> Validate()
		{
			List<IValidationError> errors = new List<IValidationError>();

			var insumosRequeridos = _uow.IngresoProduccionRepository.GetIngresoByIdConDetalles(_idIngreso).Detalles;

			if (!insumosRequeridos.Any(a => a.Producto == _value))
			{
				errors.Add(new ValidationError("PRD112_Sec0_Error_ProductoNoRequerido"));
			}

			return errors;
		}
	}
}
