using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EstacionDeTrabajoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly StockMapper _mapper;

        public EstacionDeTrabajoRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new StockMapper();
        }

        public virtual bool AnyEstacion(string idUbicacion)
        {
            return _context.T_ESTACION_TRABAJO.Any(s => s.CD_ENDERECO == idUbicacion);
        }
    }
}
