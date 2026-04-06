using WIS.Domain.DataModel;

namespace WIS.Domain.Produccion.Interfaces
{
	public interface ILogicaProduccionFactory
	{
		public ILogicaProduccion CreateLogicaProduccion(IUnitOfWork uow, string type);

		public ILogicaProduccion GetLogicaProduccion(IUnitOfWork uow, string nuIngresoProduccion);

		public ILogicaProduccion GetLogicaProduccion(IUnitOfWork uow, string idExterno, int empresa);
	}
}
