using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Produccion.Interfaces
{
	public interface IIngresoProduccionFactory
	{
		IngresoProduccion CreateIngresoProduccion(string type);
	}
}
