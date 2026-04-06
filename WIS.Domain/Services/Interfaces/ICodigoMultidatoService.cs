using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.CodigoMultidato;
using WIS.Domain.DataModel;

namespace WIS.Domain.Services.Interfaces
{
    public interface ICodigoMultidatoService
    {
        Task<CodigoMultidatoAIs> GetAIs(IUnitOfWork uow, string aplicacion, string vlCodigoMultidato, Dictionary<string, string> extraData = null, int? empresa = null, string cdCodigoMultidato = null, CodigoMultidatoTipoLectura tipoLectura = CodigoMultidatoTipoLectura.Producto);
    }
}
