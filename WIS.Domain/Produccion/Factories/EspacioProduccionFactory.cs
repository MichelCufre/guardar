using System;
using System.Collections.Generic;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Produccion.Factories
{
	public class EspacioProduccionFactory : IEspacioProduccionFactory
	{
		protected readonly List<string> _types;

		public EspacioProduccionFactory()
		{
			_types = new List<string> { CEspacioProduccion.TipoEspacioaBlackBox, CEspacioProduccion.TipoEspacioWhiteBox };
		}

		public virtual EspacioProduccion Create(string type)
		{
			if (!_types.Contains(type))
				throw new Exception("General_Sec0_Error_Error96");

			switch (type)
			{
				case CEspacioProduccion.TipoEspacioaBlackBox:
					return new EspacioBlackBox(CEspacioProduccion.TipoEspacioaBlackBox);
				case CEspacioProduccion.TipoEspacioWhiteBox:
					return new EspacioWhiteBox(CEspacioProduccion.TipoEspacioWhiteBox);
				default:
					return new EspacioBlackBox(CEspacioProduccion.TipoEspacioaBlackBox);
			}
		}
	}
}
