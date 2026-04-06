using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class RegimenAduaneroRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly RegimenAduaneroMapper _mapper;

        public RegimenAduaneroRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new RegimenAduaneroMapper();
        }

        public virtual List<RegimenAduanero> GetRegimenesAduaneros()
        {
            return this._context.T_REGIMEN_ADUANA
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual bool AnyRegimenAduanero(int cdRegimen)
        {
            return this._context.T_REGIMEN_ADUANA
                .AsNoTracking()
                .Any(d => d.CD_REGIMEN_ADUANA == cdRegimen);
        }
    }
}
