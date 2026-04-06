using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
	public class PuntajeMenorConReservaValidationRule : IValidationRule
	{
		protected readonly string _puntajeMenorConRes;
		protected readonly string _puntajeIgualConRes;
		protected readonly IFormatProvider _proveedor;

		public PuntajeMenorConReservaValidationRule(string _puntajeMenorConRes, string _puntajeIgualConRes, IFormatProvider proveedorDeFormato)
		{
			this._puntajeMenorConRes = _puntajeMenorConRes;
			this._puntajeIgualConRes = _puntajeIgualConRes;
			this._proveedor = proveedorDeFormato;
		}

		public virtual List<IValidationError> Validate()
		{
			var errors = new List<IValidationError>();
			var msgFormato = "General_Sec0_Error_Error14";
			decimal puntajeMenorConR = -1;
			decimal puntajeIgualConR = -1;

			if (string.IsNullOrEmpty(this._puntajeMenorConRes) || string.IsNullOrEmpty(this._puntajeIgualConRes))
				return errors;

			if (!decimal.TryParse(this._puntajeMenorConRes, NumberStyles.Any, _proveedor, out puntajeMenorConR) || !decimal.TryParse(this._puntajeIgualConRes, NumberStyles.Any, _proveedor, out puntajeIgualConR))
				errors.Add(new ValidationError(msgFormato));

			if (puntajeIgualConR < puntajeMenorConR)
				errors.Add(new ValidationError("General_Sec0_Error_PuntajeMenCReservaMIgualCon"));

			return errors;
		}
	}
}
