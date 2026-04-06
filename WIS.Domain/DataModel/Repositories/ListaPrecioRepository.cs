using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Facturacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ListaPrecioRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ListaPrecioMapper _mapper;

        public ListaPrecioRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ListaPrecioMapper();
        }

        #region Any
        
        public virtual bool ExisteListaPrecio(int idListaPrecio)
        {
            return this._context.T_FACTURACION_LISTA_PRECIO.Any(cb => cb.CD_LISTA_PRECIO == idListaPrecio);
        }
        
        #endregion

        #region Get
        
        public virtual ListaPrecio GetListaPrecio(int idListaPrecio)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_LISTA_PRECIO.FirstOrDefault(x => x.CD_LISTA_PRECIO == idListaPrecio));
        }

        public virtual List<ListaPrecio> GetListasPrecio()
        {
            var entities = this._context.T_FACTURACION_LISTA_PRECIO.AsNoTracking().ToList();

            List<ListaPrecio> listaPrecio = new List<ListaPrecio>();

            foreach (var entity in entities)
            {
                listaPrecio.Add(this._mapper.MapToObject(entity));
            }

            return listaPrecio;
        }
        
        #endregion

        #region Add

        public virtual void AddListaPrecio(ListaPrecio listaPrecio)
        {
            T_FACTURACION_LISTA_PRECIO entity = this._mapper.MapToEntity(listaPrecio);
            this._context.T_FACTURACION_LISTA_PRECIO.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateListaPrecio(ListaPrecio listaPrecio)
        {
            T_FACTURACION_LISTA_PRECIO entity = this._mapper.MapToEntity(listaPrecio);
            T_FACTURACION_LISTA_PRECIO attachedEntity = _context.T_FACTURACION_LISTA_PRECIO.Local
                .FirstOrDefault(w => w.CD_LISTA_PRECIO == entity.CD_LISTA_PRECIO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_LISTA_PRECIO.Attach(entity);
                _context.Entry<T_FACTURACION_LISTA_PRECIO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion
    }
}
