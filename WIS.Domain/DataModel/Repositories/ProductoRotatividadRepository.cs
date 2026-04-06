using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ProductoRotatividadRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ProductoRotatividadMapper _mapper;

        public ProductoRotatividadRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ProductoRotatividadMapper();
        }

        public virtual ProductoRotatividad GetProductoRotatividad(short codigo)
        {
            var entity = this._context.T_ROTATIVIDADE.AsNoTracking()
                .Where(d => d.CD_ROTATIVIDADE == codigo)
               .FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }

        public virtual List<ProductoRotatividad> GetProductoRotatividades()
        {
            var entities = this._context.T_ROTATIVIDADE.AsNoTracking()
               .ToList().OrderBy(x => x.CD_ROTATIVIDADE);

            var rotatividades = new List<ProductoRotatividad>();

            foreach (var entity in entities)
            {
                rotatividades.Add(this._mapper.MapToObject(entity));
            }

            return rotatividades;
        }

        public virtual bool AnyProductoRotatividad(short id)
        {
            return this._context.T_ROTATIVIDADE.Any(d => d.CD_ROTATIVIDADE == id);
        }

        public virtual List<ProductoRotatividad> GetByNombreOrCodePartial(string value)
        {
            short codigoRotatividad;
            if (short.TryParse(value, out codigoRotatividad))
            {
                return this._context.T_ROTATIVIDADE.AsNoTracking()
                    .Where(d => d.DS_ROTATIVIDADE.ToLower().Contains(value.ToLower()) || d.CD_ROTATIVIDADE == codigoRotatividad)
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }
            else
            {
                return this._context.T_ROTATIVIDADE.AsNoTracking()
                    .Where(d => d.DS_ROTATIVIDADE.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }
        }
    }
}
