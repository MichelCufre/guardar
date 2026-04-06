using System;

namespace WIS.Domain.Produccion.Models
{
	public class IngresoProduccionWhiteBox : IngresoProduccion
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
