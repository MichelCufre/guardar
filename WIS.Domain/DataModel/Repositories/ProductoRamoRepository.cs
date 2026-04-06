using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ProductoRamoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ProductoRamoMapper _mapper;
        
        public ProductoRamoRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ProductoRamoMapper();
        }

        #region Get
        public virtual ProductoRamo GetProductoRamo(short codigo)
        {
            var entity = this._context.T_RAMO_PRODUTO.AsNoTracking()
                .Where(d => d.CD_RAMO_PRODUTO == codigo)
               .FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }
        public virtual List<ProductoRamo> GetProductoRamos()
        {
            var entities = this._context.T_RAMO_PRODUTO.AsNoTracking()
               .ToList().OrderBy(x => x.CD_RAMO_PRODUTO);

            var ramos = new List<ProductoRamo>();

            foreach (var entity in entities)
            {
                ramos.Add(this._mapper.MapToObject(entity));
            }

            return ramos;
        }

        #endregion

        #region Update
        public virtual void UpdateRamoProducto(ProductoRamo ramoProducto)
        {
            T_RAMO_PRODUTO entity = this._mapper.MapToEntity(ramoProducto);
            T_RAMO_PRODUTO attachedEntity = _context.T_RAMO_PRODUTO.Local
                .FirstOrDefault(c => c.CD_RAMO_PRODUTO == entity.CD_RAMO_PRODUTO);
            
            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_RAMO_PRODUTO.Attach(entity);
                _context.Entry<T_RAMO_PRODUTO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Add
        public virtual void AddProductoRamo(ProductoRamo ramoProducto)
        {
            T_RAMO_PRODUTO entity = this._mapper.MapToEntity(ramoProducto);
            this._context.T_RAMO_PRODUTO.Add(entity);
        }
        public virtual bool AnyProductoRamo(short id)
        {
            return this._context.T_RAMO_PRODUTO.Any(d => d.CD_RAMO_PRODUTO == id);
        }
        #endregion

        #region Delete
        public virtual void DeleteRamo(short codigoRamo)
        {
            ProductoRamo ramoProducto= this.GetProductoRamo(codigoRamo);
            T_RAMO_PRODUTO entity = this._mapper.MapToEntity(ramoProducto);
            T_RAMO_PRODUTO attachedEntity = _context.T_RAMO_PRODUTO.Local
                .FirstOrDefault(c => c.CD_RAMO_PRODUTO == entity.CD_RAMO_PRODUTO);

            if (attachedEntity != null)
            {
                this._context.T_RAMO_PRODUTO.Remove(attachedEntity);
            }
            else 
            {
                this._context.T_RAMO_PRODUTO.Attach(entity);
                this._context.T_RAMO_PRODUTO.Remove(entity);
            }
        }
        #endregion
    }
}
