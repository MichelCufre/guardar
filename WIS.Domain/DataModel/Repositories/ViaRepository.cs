using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ViaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly ViaMapper _mapper;

        public ViaRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new ViaMapper();
        }

        public virtual List<Via> GetViaByNombrePartial(string nombre)
        {
            return this._context.T_VIA
                .AsNoTracking()
                .Where(d => d.CD_SITUACAO == SituacionDb.Activo 
                    && d.DS_VIA.ToLower().Contains(nombre.ToLower()))
                .Select(d => this._mapper.MapToObject(d)).ToList();
        }

        public virtual List<Via> GetVias()
        {
            return this._context.T_VIA
                .AsNoTracking()
                .Where(d => d.CD_SITUACAO == SituacionDb.Activo)
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual string GetDescripcion(string cdVia)
        {
            return this._context.T_VIA
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_VIA == cdVia)?.DS_VIA;
        }

        public virtual bool AnyVia(string cdVia)
        {
            return this._context.T_VIA
                .AsNoTracking()
                .Any(d => d.CD_VIA == cdVia);
        }
    }
}
