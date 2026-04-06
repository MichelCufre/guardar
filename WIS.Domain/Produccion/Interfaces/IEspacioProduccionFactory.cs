using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Produccion.Interfaces
{
	public interface IEspacioProduccionFactory
	{
		EspacioProduccion Create(string type);
	}
}
