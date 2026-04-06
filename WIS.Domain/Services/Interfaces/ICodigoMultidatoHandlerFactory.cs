using System.Collections.Generic;
using WIS.Domain.CodigoMultidato.Interfaces;
using WIS.Domain.DataModel;

namespace WIS.Domain.Services.Interfaces
{
    public interface ICodigoMultidatoHandlerFactory
    {
        ICodigoMultidatoHandler GetHandler(IUnitOfWork uow, string cdCodigoMultidato, string vlCodigoMultidato, out Dictionary<string, string> ais);

        ICodigoMultidatoHandler GetHandler(string cdCodigoMultidato);
    }
}
