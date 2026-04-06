using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Facturacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class CotizacionListasRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly CotizacionListasMapper _mapper;

        public CotizacionListasRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new CotizacionListasMapper();
        }

        #region Any

        public virtual bool AnyCotizacionListas(string CodigoFacturacion, string NumeroComponente, int CodigoListaPrecio)
        {
            return this._context.T_FACTURACION_LISTA_COTIZACION.Any(w => w.CD_FACTURACION.ToUpper().Trim() == CodigoFacturacion.ToUpper().Trim() && w.NU_COMPONENTE.ToUpper().Trim() == NumeroComponente.ToUpper().Trim() && w.CD_LISTA_PRECIO == CodigoListaPrecio);
        }

        #endregion

        #region Get

        public virtual CotizacionListas GetCotizacionListas(string CodigoFacturacion, string NumeroComponente, int CodigoListaPrecio)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_LISTA_COTIZACION.FirstOrDefault(w => w.CD_FACTURACION.ToUpper().Trim() == CodigoFacturacion.ToUpper().Trim() && w.NU_COMPONENTE.ToUpper().Trim() == NumeroComponente.ToUpper().Trim() && w.CD_LISTA_PRECIO == CodigoListaPrecio));
        }

        #endregion

        #region Add

        public virtual void AddCotizacionListas(CotizacionListas cotizacionListas)
        {
            T_FACTURACION_LISTA_COTIZACION entity = this._mapper.MapToEntity(cotizacionListas);
            this._context.T_FACTURACION_LISTA_COTIZACION.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateCotizacionListas(CotizacionListas cotizacionListas)
        {
            T_FACTURACION_LISTA_COTIZACION entity = this._mapper.MapToEntity(cotizacionListas);
            T_FACTURACION_LISTA_COTIZACION attachedEntity = _context.T_FACTURACION_LISTA_COTIZACION.Local.FirstOrDefault(w => w.CD_FACTURACION == entity.CD_FACTURACION && w.NU_COMPONENTE == entity.NU_COMPONENTE && w.CD_LISTA_PRECIO == entity.CD_LISTA_PRECIO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_LISTA_COTIZACION.Attach(entity);
                _context.Entry<T_FACTURACION_LISTA_COTIZACION>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
