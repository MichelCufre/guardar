using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PaisRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly PaisMapper _mapper;

        public PaisRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new PaisMapper();
        }

        public virtual Pais GetPais(string idPais)
        {
            T_PAIS pais = this._context.T_PAIS.FirstOrDefault(d => d.CD_PAIS == idPais);

            return this._mapper.MapToObject(pais);
        }

        public virtual List<Pais> GetByNombreOrIdPartial(string value)
        {
            return this._context.T_PAIS.AsNoTracking()
                .Where(d => d.DS_PAIS.ToLower().Contains(value.ToLower()) || d.CD_PAIS == value)
                .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
        }
        public virtual bool AnyPais(string value)
        {
            return this._context.T_PAIS.Any(d => d.CD_PAIS == value);
        }

        public virtual List<Pais> GetPaises()
        {
            return this._context.T_PAIS.AsNoTracking()
                .Select(p => this._mapper.MapToObject(p)).ToList();
        }

    }
}
