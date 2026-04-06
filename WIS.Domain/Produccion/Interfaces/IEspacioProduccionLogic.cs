using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Produccion.Interfaces
{
	public interface IEspacioProduccionLogic
	{
		EspacioProduccion CrearEspacioProduccion(string descripcion, string tipoEspacio, string flConfirmacionManual, string flStockConsumible, string predio);
	}
}
