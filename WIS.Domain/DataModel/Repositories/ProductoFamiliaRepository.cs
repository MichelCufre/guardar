using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ProductoFamiliaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ProductoFamiliaMapper _mapper;

        public ProductoFamiliaRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ProductoFamiliaMapper();
        }


        public virtual ProductoFamilia GetFamiliaProducto(int codigoFamilia)
        {
            var entity = this._context.T_FAMILIA_PRODUTO.AsNoTracking()
                .Where(d => d.CD_FAMILIA_PRODUTO == codigoFamilia)
               .FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }

        public virtual List<ProductoFamilia> GetProductoFamilias()
        {
            var entities = this._context.T_FAMILIA_PRODUTO.AsNoTracking()
               .ToList();

            var areas = new List<ProductoFamilia>();

            foreach (var entity in entities)
            {
                areas.Add(this._mapper.MapToObject(entity));
            }

            return areas;
        }

        public virtual List<ProductoFamilia> GetByNombreOrCodePartial(string value)
        {
            int idFamilia;
            if (int.TryParse(value, out idFamilia))
            {
                return this._context.T_FAMILIA_PRODUTO.AsNoTracking()
                    .Where(d => d.DS_FAMILIA_PRODUTO.ToLower().Contains(value.ToLower()) || d.CD_FAMILIA_PRODUTO == idFamilia)
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }
            else
            {
                return this._context.T_FAMILIA_PRODUTO.AsNoTracking()
                    .Where(d => d.DS_FAMILIA_PRODUTO.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }
        }

        public virtual void AddFamiliaProducto(ProductoFamilia familia)
        {
            var entity = this._mapper.MapToEntity(familia);

            entity.DT_ADDROW = DateTime.Now;
            entity.DT_UPDROW = DateTime.Now;

            this._context.T_FAMILIA_PRODUTO.Add(entity);
        }

        public virtual void UpdateFamiliaProducto(ProductoFamilia familia)
        {
            var entity = this._mapper.MapToEntity(familia);
            var attachedEntity = _context.T_FAMILIA_PRODUTO.Local
                .FirstOrDefault(c => c.CD_FAMILIA_PRODUTO == entity.CD_FAMILIA_PRODUTO);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_FAMILIA_PRODUTO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void RemoveFamiliaProducto(ProductoFamilia familia)
        {
            var entity = this._mapper.MapToEntity(familia);
            var attachedEntity = _context.T_FAMILIA_PRODUTO.Local
                .FirstOrDefault(c => c.CD_FAMILIA_PRODUTO == entity.CD_FAMILIA_PRODUTO);

            if (attachedEntity != null)
            {
                this._context.T_FAMILIA_PRODUTO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_FAMILIA_PRODUTO.Attach(entity);
                this._context.T_FAMILIA_PRODUTO.Remove(entity);
            }
        }

        public virtual bool AnyFamiliaProducto(int id)
        {
            return this._context.T_FAMILIA_PRODUTO.Any(d => d.CD_FAMILIA_PRODUTO == id);
        }
    }
}
