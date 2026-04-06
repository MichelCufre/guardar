using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TipoReferenciaExternaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly TipoReferenciaExternaMapper _mapper;

        public TipoReferenciaExternaRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new TipoReferenciaExternaMapper();
        }

        public virtual List<TipoReferenciaExterna> GetTiposReferenciaExterna(string tpDoc)
        {
            return this._context.T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO
                .Include("T_TIPO_REFERENCIA_EXTERNA")
                .AsNoTracking()
                .Where(td => td.TP_DOCUMENTO == tpDoc)
                .Select(d => this._mapper.MapToObject(d.T_TIPO_REFERENCIA_EXTERNA))
                .ToList();
        }

        public virtual string GetDescripcion(string tipo)
        {
            return this._context.T_TIPO_REFERENCIA_EXTERNA
                .AsNoTracking()
                .Where(d => d.TP_REFERENCIA_EXTERNA == tipo)
                .FirstOrDefault()?
                .DS_REFERENCIA_EXTERNA;
        }

        public virtual bool AnyTipoReferenciaExterna(string tipoDocumento, string tipoReferenciaExterna)
        {
            return this._context.T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO
                .Include("T_TIPO_REFERENCIA_EXTERNA")
                .AsNoTracking()
                .Any(td => td.TP_DOCUMENTO == tipoDocumento && td.TP_REFERENCIA_EXTERNA == tipoReferenciaExterna);
        }
    }
}
