using System.Linq;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class StockTraceRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;

        public StockTraceRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            // this._mapper = new StockMapper();
        }

        public virtual bool AnyStockTrace(string idUbicacion)
        {
            return _context.T_TRACE_STOCK.Any(s => s.CD_ENDERECO == idUbicacion);
        }
    }
}
