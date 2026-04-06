using System;

namespace WIS.Domain.Produccion.Models
{
	public class IngresoProduccionBlackBox : IngresoProduccion
	{
		public override void CerrarProduccion()
		{
			throw new NotImplementedException();
		}

		public override Pasada GetLatestPasada()
		{
			throw new NotImplementedException();
		}

		public override Pasada GetCurrentPasada()
		{
			throw new NotImplementedException();
		}

		public override bool IsFinalizado()
		{
			throw new NotImplementedException();
		}
	}
}
