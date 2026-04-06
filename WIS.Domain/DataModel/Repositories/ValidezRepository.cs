using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ValidezRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly ValidezMapper _mapper;

        public ValidezRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new ValidezMapper();
        }

        public virtual Validez GetValidez(string id)
        {
            return this._mapper.MapToObject(this._context.T_VALIDEZ.FirstOrDefault(x => x.CD_VALIDEZ == id));
        }

        public virtual List<Validez> GetListaValidez()
        {
            return this._context.T_VALIDEZ.ToList().Select(x => this._mapper.MapToObject(x)).ToList();
        }

    }
}
