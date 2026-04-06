using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TipoAlmacenajeSeguroRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly TipoAlmacenajeSeguroMapper _mapper;

        public TipoAlmacenajeSeguroRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new TipoAlmacenajeSeguroMapper();
        }

        public virtual string GetDescripcion(short tpAlmacenajeSeguro)
        {
            return this._context.T_TIPO_ALMACENAJE_SEGURO
                .AsNoTracking()
                .FirstOrDefault(a => a.TP_ALMACENAJE_Y_SEGURO == tpAlmacenajeSeguro)?.DS_ALMACENAJE_Y_SEGURO;
        }

        public virtual List<TipoDeAlmacenajeYSeguro> GetTipoAlmacenajeSeguroByNamePartial(string descripcion)
        {
            return this._context.T_TIPO_ALMACENAJE_SEGURO
                .AsNoTracking()
                .Where(d => d.DS_ALMACENAJE_Y_SEGURO.ToLower().Contains(descripcion.ToLower()))
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual List<TipoDeAlmacenajeYSeguro> GetTiposDeAlmacenajeYSeguro()
        {
            var tipos = new List<General.TipoDeAlmacenajeYSeguro>();
            var entries = this._context.T_TIPO_ALMACENAJE_SEGURO
                .AsNoTracking()
                .Select(s => s)
                .ToList();

            foreach (var entry in entries)
            {
                tipos.Add(this._mapper.MapToObject(entry));
            }

            return tipos;
        }

        public virtual bool AnyTipoAlmacenajeYSeguro(short tipo)
        {
            return this._context.T_TIPO_ALMACENAJE_SEGURO
                .AsNoTracking()
                .Any(d => d.TP_ALMACENAJE_Y_SEGURO == tipo);
        }
    }
}
