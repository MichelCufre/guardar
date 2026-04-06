using System;
using System.Collections.Generic;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Produccion.Factories
{
	public class IngresoProduccionFactory : IIngresoProduccionFactory
	{
		protected readonly List<string> _IngresoProduccionTypes;
		protected readonly List<string> _IngresoProduccionInstruccionTypes;

		public IngresoProduccionFactory()
		{
			_IngresoProduccionTypes = new List<string> { TipoIngresoProduccion.BlackBox, TipoIngresoProduccion.WhiteBox, TipoIngresoProduccion.Colector };
			_IngresoProduccionInstruccionTypes = new List<string> { CIngresoProduccionInstruccion.TipoValorTextoPlano, CIngresoProduccionInstruccion.TipoValorTextoEnriquecido, CIngresoProduccionInstruccion.TipoValorMarkdown, CIngresoProduccionInstruccion.TipoValorHtml };
		}

		public virtual IngresoProduccion CreateIngresoProduccion(string type)
		{
			if (!_IngresoProduccionTypes.Contains(type))
				throw new Exception("General_Sec0_Error_Error95");
			switch (type)
			{
				case TipoIngresoProduccion.BlackBox:
					return new IngresoProduccionBlackBox();
				case TipoIngresoProduccion.WhiteBox:
					return new IngresoProduccionWhiteBox();
				case TipoIngresoProduccion.Colector:
					return new IngresoProduccionColector();
				default:
					return new IngresoProduccionBlackBox();
			}
		}
	}
}
