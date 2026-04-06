using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TipoDuaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly TipoDuaMapper _mapper;

        public TipoDuaRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new TipoDuaMapper();
        }

        public virtual List<TipoDua> GetTiposDua(string tpDoc)
        {
            return this._context.T_TIPO_DUA_DOCUMENTO
                .Include("T_TIPO_DUA")
                .AsNoTracking()
                .Where(td => td.TP_DOCUMENTO == tpDoc)
                .Select(d => this._mapper.MapToObject(d.T_TIPO_DUA))
                .ToList();
        }

        public virtual string GetDescripcion(string tipo)
        {
            return this._context.T_TIPO_DUA
                .AsNoTracking()
                .Where(d => d.TP_DUA == tipo)
                .FirstOrDefault()?.DS_DUA;
        }

        public virtual bool AnyTipoDua(string tipoDocumento, string tipoDua)
        {
            return this._context.T_TIPO_DUA_DOCUMENTO
               .Include("T_TIPO_DUA")
               .AsNoTracking()
               .Any(td => td.TP_DOCUMENTO == tipoDocumento && td.TP_DUA == tipoDua);
        }
    }
}
