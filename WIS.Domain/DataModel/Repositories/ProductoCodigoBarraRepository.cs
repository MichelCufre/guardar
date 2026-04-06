using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ProductoCodigoBarraRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ProductoCodigoBarraMapper _mapper;

        public ProductoCodigoBarraRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ProductoCodigoBarraMapper();
        }

        #region CodigoBarras

        #region Add

        public virtual void AgregarCodigoBarra(ProductoCodigoBarra codigoBarras)
        {
            T_CODIGO_BARRAS entity = this._mapper.MapToEntity(codigoBarras);
            this._context.T_CODIGO_BARRAS.Add(entity);
        }

        #endregion

        #region Remove
        public virtual void DeleteCodigoBarra(string idBarra, int idEmpresa)
        {
            T_CODIGO_BARRAS entity = this._context.T_CODIGO_BARRAS
                .FirstOrDefault(x => x.CD_BARRAS == idBarra && x.CD_EMPRESA == idEmpresa);
            T_CODIGO_BARRAS attachedEntity = _context.T_CODIGO_BARRAS.Local
                .FirstOrDefault(w => w.CD_BARRAS == entity.CD_BARRAS);

            if (attachedEntity != null)
            {
                _context.T_CODIGO_BARRAS.Remove(attachedEntity);
            }
            else
            {
                _context.T_CODIGO_BARRAS.Remove(entity);
            }
        }
        #endregion

        #region Update

        public virtual void UpdateCodigoBarras(ProductoCodigoBarra codigoBarrasNuevo)
        {
            T_CODIGO_BARRAS entity = this._mapper.MapToEntity(codigoBarrasNuevo);
            T_CODIGO_BARRAS attachedEntity = _context.T_CODIGO_BARRAS.Local
                .FirstOrDefault(w => w.CD_BARRAS == entity.CD_BARRAS);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_CODIGO_BARRAS.Attach(entity);
                _context.Entry<T_CODIGO_BARRAS>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Any

        public virtual bool ExisteCodigoBarra(string cdBarras, int cdEmpresa)
        {
            return this._context.T_CODIGO_BARRAS
                .AsNoTracking()
                .Any(cb => cb.CD_BARRAS == cdBarras 
                    && cb.CD_EMPRESA == cdEmpresa);
        }
        #endregion

        #region Sequence



        #endregion

        #region Get
        public virtual ProductoCodigoBarra GetProductoCodigoBarra(string idBarra, int idEmpresa)
        {
            return this._mapper.MapToObject(this._context.T_CODIGO_BARRAS
                .Include("T_TIPO_CODIGO_BARRAS")
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_BARRAS == idBarra 
                    && x.CD_EMPRESA == idEmpresa));
        }

        #endregion

        #endregion

        #region Tipo CodigoBarras

        #region Add
        public virtual void AgregarTipoCodigoBarra(ProductoCodigoBarraTipo productoCodigoBarraTipo)
        {
            T_TIPO_CODIGO_BARRAS entity = this._mapper.MapToEntity(productoCodigoBarraTipo);
            this._context.T_TIPO_CODIGO_BARRAS.Add(entity);
        }
        #endregion

        #region Update
        public virtual void UpdateTipoCodigoBarras(ProductoCodigoBarraTipo productoCodigoBarraTipo)
        {
            T_TIPO_CODIGO_BARRAS entity = this._mapper.MapToEntity(productoCodigoBarraTipo);
            T_TIPO_CODIGO_BARRAS attachedEntity = _context.T_TIPO_CODIGO_BARRAS.Local
                .FirstOrDefault(w => w.TP_CODIGO_BARRAS == entity.TP_CODIGO_BARRAS);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_TIPO_CODIGO_BARRAS.Attach(entity);
                _context.Entry<T_TIPO_CODIGO_BARRAS>(entity).State = EntityState.Modified;
            }
        }
        #endregion

        #region Remove
        public virtual void DeleteTipoCodigoBarra(int idCodigoBarrasTipo)
        {
            T_TIPO_CODIGO_BARRAS entity = this._context.T_TIPO_CODIGO_BARRAS
                .FirstOrDefault(x => x.TP_CODIGO_BARRAS == idCodigoBarrasTipo);
            T_TIPO_CODIGO_BARRAS attachedEntity = _context.T_TIPO_CODIGO_BARRAS.Local
                .FirstOrDefault(w => w.TP_CODIGO_BARRAS == entity.TP_CODIGO_BARRAS);

            if (attachedEntity != null)
            {
                _context.T_TIPO_CODIGO_BARRAS.Remove(attachedEntity);
            }
            else
            {
                _context.T_TIPO_CODIGO_BARRAS.Remove(entity);
            }
        }
        #endregion

        #region GET
        public virtual ProductoCodigoBarraTipo GetProductoCodigoBarraTipo(int idCodigoBarrasTipo)
        {
            return this._mapper.MapToObject(this._context.T_TIPO_CODIGO_BARRAS.FirstOrDefault(x => x.TP_CODIGO_BARRAS == idCodigoBarrasTipo));
        }

        public virtual List<ProductoCodigoBarraTipo> GetTiposCodigosBarras()
        {
            List<T_TIPO_CODIGO_BARRAS> listaTiposCB = _context.T_TIPO_CODIGO_BARRAS.AsNoTracking().Select(d => d).ToList();
            List<ProductoCodigoBarraTipo> listaRetorno = new List<ProductoCodigoBarraTipo>();

            foreach (var tipo in listaTiposCB)
            {
                listaRetorno.Add(this._mapper.MapToObject(tipo));
            }
            return listaRetorno;

        }
        #endregion

        #region ANY
        public virtual bool ExisteTipoCodigoBarras(int tpCodigoBarras)
        {
            return _context.T_TIPO_CODIGO_BARRAS.Any(x => x.TP_CODIGO_BARRAS == tpCodigoBarras);
        }
        #endregion

        #endregion
    }
}
