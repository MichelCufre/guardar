using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
	public class PuntajeMenorSinReservaValidationRule : IValidationRule
	{
		protected readonly string _puntajeMenorSinRes;
		protected readonly string _puntajeIgualSinRes;
		protected readonly IFormatProvider _proveedor;

		public PuntajeMenorSinReservaValidationRule(string _puntajeMenorSinRes, string _puntajeIgualSinRes, IFormatProvider proveedorDeFormato)
		{
			this._puntajeMenorSinRes = _puntajeMenorSinRes;
			this._puntajeIgualSinRes = _puntajeIgualSinRes;
			this._proveedor = proveedorDeFormato;
		}

		public virtual List<IValidationError> Validate()
		{
			var errors = new List<IValidationError>();
			var msgFormato = "General_Sec0_Error_Error14";
			decimal puntajeMenorSinR = -1;
			decimal puntajeIgualSinR = -1;

			if (string.IsNullOrEmpty(this._puntajeMenorSinRes) || string.IsNullOrEmpty(this._puntajeIgualSinRes))
				return errors;

			if (!decimal.TryParse(this._puntajeMenorSinRes, NumberStyles.Any, _proveedor, out puntajeMenorSinR) || !decimal.TryParse(this._puntajeIgualSinRes, NumberStyles.Any, _proveedor, out puntajeIgualSinR))
				errors.Add(new ValidationError(msgFormato));

			if (puntajeIgualSinR < puntajeMenorSinR)
				errors.Add(new ValidationError("General_Sec0_Error_PuntajeMenSReservaMIgualSin"));

			return errors;
		}
	}
}
